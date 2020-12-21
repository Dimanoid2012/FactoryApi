using System;
using System.Collections.Generic;
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
    [Authorize(Roles = Roles.Administrator)]
    public class SizesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _connectionString;
        private readonly ILogger<SizesController> _logger;

        public SizesController(ApplicationContext context, Configuration configuration,
            ILogger<SizesController> logger)
        {
            _context = context;
            _connectionString = configuration.ConnectionString;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<SizeDto>> GetSizes()
        {
            await using var db = new NpgsqlConnection(_connectionString);
            return await db.QueryAsync<SizeDto>(@"
                SELECT ""Id"", ""Name"", ""Value""
                FROM ""Sizes""");
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateSize(SizeDto dto)
        {
            if (dto.Name == null)
                return BadRequest("Не указано наименование размера");
            if (dto.Value == null)
                return BadRequest("Не указано обозначение размера");

            var size = new Size(dto.Name, dto.Value);
            
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал размер с id {size.Id}");
            return size.Id;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(Guid id)
        {
            _context.Sizes.Remove(new Size(id));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    $"Ошибка удаления несуществующего размера с id {id} пользователем {User.Identity?.Name}: {ex}");
                return NotFound($"Не найден размер с id {id}");
            }

            return NoContent();
        }
    }
}
