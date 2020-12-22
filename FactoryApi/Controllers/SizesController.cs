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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class SizesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<SizesController> _logger;

        public SizesController(ApplicationContext context, ILogger<SizesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Список всех размеров
        /// </summary>
        /// <response code="200">Возвращает список всех размеров</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwaggerDoc.Size>>> GetSizes()
        {
            return await _context.Sizes
                .Select(x => new SwaggerDoc.Size
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value
                })
                .ToListAsync();
        }

        /// <summary>
        /// Создание нового размера
        /// </summary>
        /// <param name="dto">Параметры нового размера</param>
        /// <response code="200">Размер успешно создан. Возвращает идентификатор созданного размера</response>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateSize(SizeDto dto)
        {
            var size = new Size(dto.Name, dto.Value);
            
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал размер с id {size.Id}");
            return size.Id;
        }

        /// <summary>
        /// Удаляет размер с указанным идентификатором
        /// </summary>
        /// <param name="id" example="fd058e3f-a5e0-47ef-bf15-3d83edc87a61">Идентификатор размера</param>
        /// <response code="204">Размер успешно удален. Ничего не возвращает</response>
        /// <response code="404">Размер не найден. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
