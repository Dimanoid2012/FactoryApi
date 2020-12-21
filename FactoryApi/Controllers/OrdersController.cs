using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using FactoryApi.DTO;
using FactoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly bool _enableWriting;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApplicationContext context, Configuration configuration,
            ILogger<OrdersController> logger)
        {
            _context = context;
            _enableWriting = configuration.EnableWriting;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Reception, Issuer")]
        public async Task<IActionResult> GetOrders(string? search = null)
        {
            var query = _context.Orders.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.Number.ToString().Contains(search)
                    || x.ClientName.Contains(search)
                    || x.ClientPhone.Contains(search));
            if (User.IsInRole(Roles.Reception))
                query = query.Where(x => x.State == OrderState.Confirming);
            else if (User.IsInRole(Roles.Issuer))
                query = query.Where(x => x.State == OrderState.Issue);

            return Ok(await query.Select(x => new
            {
                x.Id,
                x.Number,
                x.State,
                Model = new
                {
                    x.Model.Id,
                    x.Model.Name,
                    Color = new
                    {
                        x.Model.Color.Id,
                        x.Model.Color.Name,
                        Value = x.Model.Color.RGB.ToString()
                    }
                },
                Size = new
                {
                    x.Size.Id,
                    x.Size.Name,
                    x.Size.Value
                },
                x.Side,
                x.ClientName,
                x.ClientPhone
            }).ToListAsync());
        }
        
        [HttpGet("forBoard")]
        [Authorize(Roles = Roles.Board)]
        public async Task<IActionResult> GetOrdersForBoard()
        {
            var query = _context.Orders.AsNoTracking()
                .Where(x => !new[] {OrderState.Done, OrderState.Canceled}.Contains(x.State))
                .Select(x => new
                {
                    x.Id,
                    x.Number,
                    x.State
                });

            return Ok(await query.ToListAsync());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> CreateOrder(NewOrderDto dto)
        {
            var model = await _context.Models.FindAsync(dto.ModelId);
            if (model == null)
                return BadRequest("Не указана модель");

            var size = await _context.Sizes.FindAsync(dto.SizeId);
            if (size == null)
                return BadRequest("Не указан размер");

            var image = await _context.Images.FindAsync(dto.ImageId);
            if (image == null)
                return BadRequest("Не указан принт");

            Order order;
            try
            {
                order = new Order(model, size, dto.Side, image, dto.Top, dto.Left, dto.ClientName, dto.ClientPhone);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogWarning($"Ошибка создания нового заказа пользователем {User.Identity?.Name}: {ex}");
                return NotFound($"Не удалось создать заказ: {ex.Message}");
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Создан новый заказ {order.Id} пользователем {User.Identity?.Name}");

            return order.Id;

        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = Roles.Reception)]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var order = await _context.Orders.AsNoTracking()
                .Include(x => x.Image)
                .Include(x => x.Model).ThenInclude(x => x.Color)
                .Include(x => x.Size)
                .Where(x => x.State == OrderState.Confirming)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound($"Не найден заказ с id {id} в статусе ПОДТВЕРЖДЕНИЕ");

            _context.Entry(order).State = EntityState.Modified;
            if (!order.Cancel())
            {
                _logger.LogWarning($"Ошибка отмены заказа {id} пользователем {User.Identity?.Name}");
                return BadRequest("Заказ не находится в статусе ПОДТВЕРЖДЕНИЕ");
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Заказ {id} отменен пользователем {User.Identity?.Name}");

            return Ok(new
            {
                order.Id,
                order.Number,
                order.Side,
                Image = new
                {
                    order.Image.Width,
                    order.Image.Height,
                    order.Image.Type,
                    ContentsBase64 = Convert.ToBase64String(order.Image.Contents)
                },
                Model = new
                {
                    order.Model.Id,
                    order.Model.Name,
                    Color = new
                    {
                        order.Model.Color.Id,
                        order.Model.Color.Name,
                        Value = order.Model.Color.RGB.ToString()
                    }
                },
                Size = new
                {
                    order.Size.Id,
                    order.Size.Name,
                    order.Size.Value
                }
            });
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Roles = Roles.Reception)]
        public async Task<IActionResult> ConfirmOrder(Guid id)
        {
            var order = await _context.Orders.AsNoTracking()
                .Where(x => x.State == OrderState.Confirming)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound($"Не найден заказ с id {id} в статусе ПОДТВЕРЖДЕНИЕ");

            _context.Entry(order).State = EntityState.Modified;
            if (!order.Confirm())
            {
                _logger.LogWarning($"Ошибка подтверждения заказа пользователем {User.Identity?.Name}");
                return BadRequest("Заказ не находится в статусе ПОДТВЕРЖДЕНИЕ");
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Заказ {id} переведен в статус {(_enableWriting ? "НА НАНЕСЕНИИ" : "НА ПЕЧАТИ")} пользователем {User.Identity?.Name}");

            return Ok("Заказ переведен в статус " + (_enableWriting ? "НА НАНЕСЕНИИ" : "НА ПЕЧАТИ"));
        }

        [HttpPost("getTask")]
        [Authorize(Roles = "Writer, Printer")]
        public async Task<IActionResult> GetTask()
        {
            Expression<Func<Order, bool>> searchExpr;
            var isWriter = User.IsInRole(Roles.Writer);
            if (isWriter)
                searchExpr = x => x.State == OrderState.Writing && x.WriterName == null;
            else
                searchExpr = x => x.State == OrderState.Printing && x.PrinterName == null;

            var order = await _context.Orders.AsNoTracking()
                .Include(x => x.Image)
                .Include(x => x.Model).ThenInclude(x => x.Color)
                .Include(x => x.Size)
                .Where(searchExpr)
                .FirstOrDefaultAsync();
            if (order == null)
                return NotFound($"Не найден свободный заказ в статусе {(isWriter ? "НА НАНЕСЕНИИ" : "НА ПЕЧАТИ")}");

            _context.Entry(order).State = EntityState.Modified;

            var username = User.Identity?.Name ?? "";
            var result = isWriter ? order.SetWriter(username) : order.SetPrinter(username);
            if (!result)
            {
                _logger.LogWarning($"Ошибка принятия в работу заказа пользователем {username}");
                return BadRequest("Ошибка принятия в работу заказа");
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Заказ {order.Id} принят в работу пользователем {username}");

            return Ok(new
            {
                order.Id,
                order.Number,
                order.Side,
                Image = new
                {
                    order.Image.Width,
                    order.Image.Height,
                    order.Image.Type,
                    ContentsBase64 = Convert.ToBase64String(order.Image.Contents)
                },
                order.Top,
                order.Left,
                Model = new
                {
                    order.Model.Id,
                    order.Model.Name,
                    Color = new
                    {
                        order.Model.Color.Id,
                        order.Model.Color.Name,
                        Value = order.Model.Color.RGB.ToString()
                    }
                },
                Size = new
                {
                    order.Size.Id,
                    order.Size.Name,
                    order.Size.Value
                }
            });
        }

        [HttpPost("{id}/completeTask")]
        [Authorize(Roles = "Writer, Printer")]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            var username = User.Identity?.Name ?? "";
            var order = await _context.Orders.AsNoTracking()
                .Where(x => x.State == OrderState.Writing && x.WriterName == username
                            || x.State == OrderState.Printing && x.PrinterName == username)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound($"Не найден заказ с id {id}");

            _context.Entry(order).State = EntityState.Modified;
            if (!order.Complete())
            {
                _logger.LogWarning($"Ошибка перевода заказа в статус ВЫДАЧА пользователем {username}");
                return BadRequest("Не удалось перевести заказ в статус ВЫДАЧА");
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Заказ {id} переведен в статус ВЫДАЧА пользователем {username}");

            return Ok("Заказ переведен в статус ВЫДАЧА");
        }

        [HttpPost("{id}/issue")]
        [Authorize(Roles = Roles.Issuer)]
        public async Task<IActionResult> IssueOrder(Guid id)
        {
            var order = await _context.Orders.AsNoTracking()
                .Where(x => x.State == OrderState.Issue)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound($"Не найден заказ с id {id} в статусе ВЫДАЧА");

            _context.Entry(order).State = EntityState.Modified;
            if (!order.Issue())
            {
                _logger.LogWarning($"Ошибка выдачи заказа пользователем {User.Identity?.Name}");
                return BadRequest("Заказ не находится в статусе ВЫДАЧА");
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Заказ {id} переведен в статус ЗАВЕРШЕНО пользователем {User.Identity?.Name}");

            return Ok("Заказ переведен в статус ЗАВЕРШЕНО");
        }
    }
}