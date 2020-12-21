﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ILogger<OrdersController>  _logger;
        public OrdersController(ApplicationContext context, Configuration configuration, ILogger<OrdersController> logger)
        {
            _context = context;
            _enableWriting = configuration.EnableWriting;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(string? search = null)
        {
            var query = _context.Orders.AsNoTracking();
            if (User.IsInRole(Roles.Reception))
            {
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(x =>
                        x.Number.ToString().Contains(search)
                        || x.ClientName.Contains(search)
                        || x.ClientPhone.Contains(search));

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

            return Ok(await query.Select(x => new
            {
                Id = x.Id,
                Number = x.Number,
                State = x.State,
                ModelId = x.Model.Id,
                SizeId = x.Size.Id,
                Side = x.Side,
                ImageId = x.Image.Id,
                Top = x.Top,
                Left = x.Left,
                ClientName = x.ClientName,
                ClientPhone = x.ClientPhone
            }).ToListAsync());
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
    }
}
