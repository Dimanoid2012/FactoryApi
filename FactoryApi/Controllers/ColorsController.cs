using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
            var colors =  await db.QueryAsync<ColorInner>(@"
                SELECT ""Id"", ""Name"", ""RGB_R"" ""R"", ""RGB_G"" ""G"", ""RGB_B"" ""B""
                FROM ""Colors""");
            return colors.Select(x => new ColorDto {Id = x.Id, Name = x.Name, Value = x.ToString()});
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateColor(ColorDto dto)
        {
            if (dto.Name == null)
                return BadRequest("Не указано наименование цвета");
            if (dto.Value == null)
                return BadRequest("Не указано значение цвета");
            RGB rgb;
            try
            {
                rgb = RGB.Parse(dto.Value);
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"Ошибка создания нового цвета пользователем {User.Identity?.Name}: {ex}");
                return BadRequest(ex.Message);
            }

            var color = new Color(dto.Name, rgb);

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал цвет с id {color.Id}");
            return color.Id;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(Guid id)
        {
            _context.Colors.Remove(new Color(id));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning($"Ошибка удаления несуществующего цвета с id {id} пользователем {User.Identity?.Name}: {ex}");
                return NotFound($"Не найден цвет с id {id}");
            }

            return NoContent();
        }
    }

    internal class ColorInner
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public short R { get; set; }
        public short G { get; set; }
        public short B { get; set; }
        
        /// <summary>
        /// Возвращает HTML-представление цвета, например, #FFFFFF.
        /// </summary>
        /// <returns>Возвращает HTML-представление цвета, например, #FFFFFF.</returns>
        public override string ToString() => $"#{R:X2}{G:X2}{B:X2}";
    }
}
