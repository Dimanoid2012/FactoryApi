using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FactoryApi.DTO;
using FactoryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _connectionString;
        private readonly ILogger<ColorsController>  _logger;
        
        public ColorsController(ApplicationContext context, ConnectionString connectionString, ILogger<ColorsController> logger)
        {
            _context = context;
            _connectionString = connectionString.Value;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<ColorDto>> GetColors()
        {
            await using var db = new NpgsqlConnection(_connectionString);
            var colors =  await db.QueryAsync<Color>(@"
                SELECT ""Name"", ""R"", ""G"", ""B""
                FROM ""Colors""");
            return colors.Select(x => new ColorDto {Name = x.Name, Value = x.ToString()});
        }

        [HttpPost]
        public async Task<IActionResult> CreateColor(ColorDto dto)
        {
            if (dto.Name == null)
                return BadRequest("Не указано наименование цвета");
            if (dto.Value == null)
                return BadRequest("Не указано значение цвета");
            Color color;
            try
            {
                color = Color.Parse(dto.Name, dto.Value);
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"Ошибка создания нового цвета пользователем {User.Identity?.Name}: {ex}");
                return BadRequest(ex.Message);
            }

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал цвет с id {color.Id}");
            return Ok(color.Id);
        }
    }
}
