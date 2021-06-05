using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FactoryApi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model = FactoryApi.Models.Model;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class ModelsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ModelsController>  _logger;
        
        public ModelsController(ApplicationContext context, ILogger<ModelsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        /// <summary>
        /// Список всех моделей
        /// </summary>
        /// <response code="200">Возвращает список всех моделей</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwaggerDoc.Model>>> GetModels()
        {
            return await _context.Models.AsNoTracking().Select(x => new SwaggerDoc.Model
            {
                Id = x.Id,
                Name = x.Name,
                Color = new SwaggerDoc.Color
                {
                    Id = x.Color.Id,
                    Name = x.Color.Name,
                    Value = x.Color.RGB.ToString()
                }
            }).ToListAsync();
        }

        /// <summary>
        /// Создание новой модели
        /// </summary>
        /// <param name="dto">Параметры новой модели</param>
        /// <response code="200">Модель успешно создана. Возвращает идентификатор созданной модели</response>
        /// <response code="400">Не найден цвет модели. Возвращает текст ошибки</response>
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

        /// <summary>
        /// Удаляет модель с указанным идентификатором
        /// </summary>
        /// <param name="id" example="fd058e3f-a5e0-47ef-bf15-3d83edc87a61">Идентификатор модели</param>
        /// <response code="204">Модель успешно удалена. Ничего не возвращает</response>
        /// <response code="404">Модель не найдена. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(Guid id)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
                return NotFound();
            _context.Models.Remove(model);
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
