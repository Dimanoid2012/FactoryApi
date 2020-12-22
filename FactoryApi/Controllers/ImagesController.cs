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
    public class ImagesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ApplicationContext context, ILogger<ImagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Список всех картинок
        /// </summary>
        /// <response code="200">Возвращает список всех картинок</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwaggerDoc.Image>>> GetSizes()
        {
            return await _context.Images.Select(x => new SwaggerDoc.Image
            {
                Id = x.Id,
                Name = x.Name,
                Width = x.Width,
                Height = x.Height,
                Type = x.Type,
                ContentsBase64 = Convert.ToBase64String(x.Contents)
            }).ToListAsync();
        }

        /// <summary>
        /// Создание новой картинки
        /// </summary>
        /// <param name="dto">Параметры новой картинки</param>
        /// <response code="200">Картинки успешно создана. Возвращает идентификатор созданной картинки</response>
        /// <response code="400">Содержимое картинки не похоже на base64. Возвращает текст ошибки</response>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateImage(ImageDto dto)
        {
            byte[] contents;
            try
            {
                contents = Convert.FromBase64String(dto.ContentsBase64);
            }
            catch (FormatException)
            {
                return BadRequest("Неправильный формат base64");
            }

            var image = new Image(dto.Name, dto.Width, dto.Height, dto.Type, contents);
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал картинку с id {image.Id}");
            return image.Id;
        }

        /// <summary>
        /// Удаляет картинку с указанным идентификатором
        /// </summary>
        /// <param name="id" example="fd058e3f-a5e0-47ef-bf15-3d83edc87a61">Идентификатор картинки</param>
        /// <response code="204">Картинка успешно удалена. Ничего не возвращает</response>
        /// <response code="404">Картинка не найдена. Возвращает текст ошибки</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            _context.Images.Remove(new Image(id));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    $"Ошибка удаления несуществующей картинки с id {id} пользователем {User.Identity?.Name}: {ex}");
                return NotFound($"Не найдена картинка с id {id}");
            }

            return NoContent();
        }
    }
}
