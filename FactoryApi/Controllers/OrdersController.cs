using System;
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
        private readonly string _connectionString;
        private readonly ILogger<OrdersController>  _logger;
        public OrdersController(ApplicationContext context, ConnectionString connectionString, ILogger<OrdersController> logger)
        {
            _context = context;
            _connectionString = connectionString.Value;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<OrderDto>> GetSizes()
        {
            await using var db = new NpgsqlConnection(_connectionString);
            return await db.QueryAsync<OrderDto>(@"
                SELECT ""Id"", ""State"", ""Number"", ""ModelId"", ""SizeId"", ""Side"", ""ImageId"", ""Top"", ""Left"", ""ClientName"", ""ClientPhone""
                FROM ""Orders""");
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

            return order.Id;

        }
    }
}
