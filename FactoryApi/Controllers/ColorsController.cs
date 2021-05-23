using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Работа со справочником цветов
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class ColorsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ColorsController> _logger;

        public ColorsController(ApplicationContext context, ILogger<ColorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Список всех цветов
        /// </summary>
        /// <response code="200">Возвращает список всех цветов</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SwaggerDoc.Color>>> GetColors()
        {
            var colors = await _context.Colors.AsNoTracking().ToListAsync();
            return colors.Select(x => new SwaggerDoc.Color {Id = x.Id, Name = x.Name, Value = x.RGB.ToString()}).ToList();
        }

        /// <summary>
        /// Создание нового цвета
        /// </summary>
        /// <param name="dto">Параметры нового цвета</param>
        /// <response code="200">Цвет успешно создан. Возвращает идентификатор созданного цвета</response>
        /// <response code="400">Значение цвета в неправильном формате. Возвращает текст ошибки</response>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateColor(ColorDto dto)
        {
            RGB rgb;
            try
            {
                rgb = RGB.Parse(dto.Value);
            }
            catch (Exception ex)
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

        /// <summary>
        /// Удаляет цвет с указанным идентификатором
        /// </summary>
        /// <param name="id" example="fd058e3f-a5e0-47ef-bf15-3d83edc87a61">Идентификатор цвета</param>
        /// <response code="204">Цвет успешно удален. Ничего не возвращает</response>
        /// <response code="404">Цвет не найден. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
                _logger.LogWarning(
                    $"Ошибка удаления несуществующего цвета с id {id} пользователем {User.Identity?.Name}: {ex}");
                return NotFound($"Не найден цвет с id {id}");
            }

            return NoContent();
        }
    }
}
