using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FactoryApi.DTO;
using FactoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// Список заказов. Для администратора возвращаются все заказы, для ресепшена - только заказы в статусе ПОДТВЕРЖДЕНИЕ, для оператора выдачи - только заказы в статусе ВЫДАЧА.
        /// Доступно только ролям Администратор, Ресепшен, Оператор выдачи
        /// </summary>
        /// <param name="search">Строка поиска по номеру заказа/имени клиента/номеру телефона клиента</param>
        /// <response code="200">Возвращает список заказов</response>
        [HttpGet]
        [Authorize(Roles = "Administrator, Reception, Issuer")]
        public async Task<ActionResult<IEnumerable<SwaggerDoc.Order>>> GetOrders(string? search = null)
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

            return await query.Select(x => new SwaggerDoc.Order
            {
                Id = x.Id,
                Number = x.Number,
                State = x.State,
                Side = x.Side,
                Image = new SwaggerDoc.Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Width = x.Image.Width,
                    Height = x.Image.Height,
                    Type = x.Image.Type,
                    ContentsBase64 = Convert.ToBase64String(x.Image.Contents)
                },
                Model = new SwaggerDoc.Model
                {
                    Id = x.Model.Id,
                    Name = x.Model.Name,
                    Color = new SwaggerDoc.Color
                    {
                        Id = x.Model.Color.Id,
                        Name = x.Model.Color.Name,
                        Value = x.Model.Color.RGB.ToString()
                    }
                },
                Size = new SwaggerDoc.Size
                {
                    Id = x.Size.Id,
                    Name = x.Size.Name,
                    Value = x.Size.Value
                },
                ClientName = x.ClientName,
                ClientPhone = x.ClientPhone
            }).ToListAsync();
        }

        /// <summary>
        /// Список заказов для табло. 
        /// Доступно только роли Табло.
        /// </summary>
        /// <response code="200">Возвращает список заказов</response>
        [HttpGet("forBoard")]
        [Authorize(Roles = Roles.Board)]
        public async Task<ActionResult<IEnumerable<SwaggerDoc.OrderBoard>>> GetOrdersForBoard()
        {
            return await _context.Orders.AsNoTracking()
                .Where(x => !new[] {OrderState.Done, OrderState.Canceled}.Contains(x.State))
                .Select(x => new SwaggerDoc.OrderBoard
                {
                    Id = x.Id,
                    Number = x.Number,
                    State = x.State
                }).ToListAsync();
        }

        /// <summary>
        /// Создание нового заказа
        /// </summary>
        /// <param name="dto">Параметры нового заказа</param>
        /// <response code="200">Заказ успешно создан. Возвращает идентификатор созданного заказа</response>
        /// <response code="400">Не найдены модель, размер или картинка; неправильно указано расположение принта; указаны отрицательные смещения. Возвращает текст ошибки</response>
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

        /// <summary>
        /// Отменяет заказ.
        /// Доступно только для роли Ресепшен.
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <response code="200">Заказ успешно отменен. Возвращает информацию о заказе</response>
        /// <response code="400">Ошибка отмены заказа. Возвращает текст ошибки</response>
        /// <response code="404">Не найден заказ. Возвращает текст ошибки</response>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = Roles.Reception)]
        public async Task<ActionResult<SwaggerDoc.Order>> CancelOrder(Guid id)
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

            return new SwaggerDoc.Order
            {
                Id = order.Id,
                State = order.State,
                Number = order.Number,
                Side = order.Side,
                Image = new SwaggerDoc.Image
                {
                    Id = order.Image.Id,
                    Name = order.Image.Name,
                    Width = order.Image.Width,
                    Height = order.Image.Height,
                    Type = order.Image.Type,
                    ContentsBase64 = Convert.ToBase64String(order.Image.Contents)
                },
                Model = new SwaggerDoc.Model
                {
                    Id = order.Model.Id,
                    Name = order.Model.Name,
                    Color = new SwaggerDoc.Color
                    {
                        Id = order.Model.Color.Id,
                        Name = order.Model.Color.Name,
                        Value = order.Model.Color.RGB.ToString()
                    }
                },
                Size = new SwaggerDoc.Size
                {
                    Id = order.Size.Id,
                    Name = order.Size.Name,
                    Value = order.Size.Value
                }
            };
        }

        /// <summary>
        /// Подтверждает заказ заказ.
        /// Доступно только для роли Ресепшен.
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <response code="200">Заказ успешно подтвержден. Возвращает текст подтверждения</response>
        /// <response code="400">Ошибка подтверждения заказа. Возвращает текст ошибки</response>
        /// <response code="404">Не найден заказ. Возвращает текст ошибки</response>
        [HttpPost("{id}/confirm")]
        [Authorize(Roles = Roles.Reception)]
        public async Task<ActionResult<string>> ConfirmOrder(Guid id)
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

            return "Заказ переведен в статус " + (_enableWriting ? "НА НАНЕСЕНИИ" : "НА ПЕЧАТИ");
        }

        /// <summary>
        /// Запрашивает заказ нв выполнение и устанавливает исполнителя.
        /// Доступно только для ролей МАСТЕР НАНЕСЕНИЯ, МАСТЕР ПЕЧАТИ.
        /// </summary>
        /// <response code="200">Возвращает заказ</response>
        /// <response code="204">Нет заказа на выполнение</response>
        /// <response code="400">Ошибка принятия заказа в работу. Возвращает текст ошибки</response>
        [HttpPost("getTask")]
        [Authorize(Roles = "Writer, Printer")]
        public async Task<ActionResult<SwaggerDoc.Order>> GetTask()
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
                return NoContent();

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

            return new SwaggerDoc.Order
            {
                Id = order.Id,
                State = order.State,
                Number = order.Number,
                Side = order.Side,
                Image = new SwaggerDoc.Image
                {
                    Id = order.Image.Id,
                    Name = order.Image.Name,
                    Width = order.Image.Width,
                    Height = order.Image.Height,
                    Type = order.Image.Type,
                    ContentsBase64 = Convert.ToBase64String(order.Image.Contents)
                },
                Top = order.Top,
                Left = order.Left,
                Model = new SwaggerDoc.Model
                {
                    Id = order.Model.Id,
                    Name = order.Model.Name,
                    Color = new SwaggerDoc.Color
                    {
                        Id = order.Model.Color.Id,
                        Name = order.Model.Color.Name,
                        Value = order.Model.Color.RGB.ToString()
                    }
                },
                Size = new SwaggerDoc.Size
                {
                    Id = order.Size.Id,
                    Name = order.Size.Name,
                    Value = order.Size.Value
                }
            };
        }

        /// <summary>
        /// Завершает работу с заказом, переводя его в статус ВЫДАЧА
        /// Доступно только для ролей МАСТЕР НАНЕСЕНИЯ, МАСТЕР ПЕЧАТИ.
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <response code="204">Работа с заказом успешно завершена. Ничего не возвращает</response>
        /// <response code="400">Ошибка завершения заказа. Возвращает текст ошибки</response>
        /// <response code="404">Заказ не найден. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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

            return NoContent();
        }

        /// <summary>
        /// Выдает заказ клиенту, переводя его в статус ЗАВЕРШЕНО.
        /// Доступно только для роли ОПЕРАТОР ВЫДАЧИ.
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <response code="204">Работа с заказом успешно завершена. Ничего не возвращает</response>
        /// <response code="400">Ошибка завершения заказа. Возвращает текст ошибки</response>
        /// <response code="404">Заказ не найден. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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

            return NoContent();
        }

        /// <summary>
        /// Изменяет статус заказа.
        /// Доступно только для роли АДМИНИСТРАТОР.
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="state">Состояние заказа</param>
        /// <response code="204">Статус заказа успешно изменен. Ничего не возвращает</response>
        /// <response code="404">Заказ не найден. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("{id}/changeState")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> ChangeOrderState(Guid id, OrderState state)
        {
            var order = await _context.Orders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound($"Не найден заказ с id {id}");

            _context.Entry(order).State = EntityState.Modified;

            order.State = state;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Заказ {id} переведен в статус {order.StateText()} пользователем {User.Identity?.Name}");

            return NoContent();
        }

        /// <summary>
        /// Удаляет заказ с указанным идентификатором.
        /// Доступно только для роли АДМИНИСТРАТОР.
        /// </summary>
        /// <param name="id" example="fd058e3f-a5e0-47ef-bf15-3d83edc87a61">Идентификатор заказа</param>
        /// <response code="204">Заказ успешно удален. Ничего не возвращает</response>
        /// <response code="404">Заказ не найден. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            _context.Entry(new Order(id)).State = EntityState.Deleted;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound($"Не найден заказ с id {id}");
            }

            _logger.LogInformation(
                $"Заказ {id} удален пользователем {User.Identity?.Name}");

            return NoContent();
        }
        
        /// <summary>
        /// Удаляет все заказы.
        /// Доступно только для роли АДМИНИСТРАТОР.
        /// </summary>
        /// <response code="204">Заказ успешно удален. Ничего не возвращает</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("deleteAll")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteAllOrders()
        {
            var orders = await _context.Orders.ToListAsync();

            _context.Orders.RemoveRange(orders);

            await _context.SaveChangesAsync(); 
            
            _logger.LogInformation(
                $"Все заказы удалены пользователем {User.Identity?.Name}");

            return NoContent();
        }
    }
}