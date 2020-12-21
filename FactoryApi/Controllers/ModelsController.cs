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
    [Authorize(Roles = Roles.Administrator)]
    public class ModelsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _connectionString;
        private readonly ILogger<ModelsController>  _logger;
        
        public ModelsController(ApplicationContext context, Configuration configuration, ILogger<ModelsController> logger)
        {
            _context = context;
            _connectionString = configuration.ConnectionString;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<ModelDto>> GetModels()
        {
            await using var db = new NpgsqlConnection(_connectionString);
            return  await db.QueryAsync<ModelDto>(@"
                SELECT ""Id"", ""Name"", ""ColorId""
                FROM ""Models""");
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateModel(ModelDto dto)
        {
            var color = await _context.Colors.FindAsync(dto.ColorId);
            if (color == null)
                return BadRequest("Не указан цвет");
            
            Model model;
            try
            {
                model = new Model(dto.Name, color);
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"Ошибка создания новой модели пользователем {User.Identity?.Name}: {ex}");
                return BadRequest(ex.Message);
            }
            
            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал модель с id {model.Id}");
            return model.Id;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(Guid id)
        {
            _context.Models.Remove(new Model(id));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning($"Ошибка удаления несуществующей модели с id {id} пользователем {User.Identity?.Name}: {ex}");
                return NotFound($"Не найдена модель с id {id}");
            }

            return NoContent();
        }
    }
}
