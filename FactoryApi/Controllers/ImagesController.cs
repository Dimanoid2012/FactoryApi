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
    public class ImagesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _connectionString;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ApplicationContext context, ConnectionString connectionString,
            ILogger<ImagesController> logger)
        {
            _context = context;
            _connectionString = connectionString.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ImageDto>> GetSizes()
        {
            await using var db = new NpgsqlConnection(_connectionString);
            var images =  await db.QueryAsync<ImageInner>(@"
                SELECT ""Id"", ""Name"", ""Width"", ""Height"", ""Type"", ""Contents""
                FROM ""Images""");
            return images.Select(x => new ImageDto
            {
                Id = x.Id,
                Name = x.Name,
                Width = x.Width,
                Height = x.Height,
                Type = x.Type,
                ContentsBase64 = Convert.ToBase64String(x.Contents)
            });
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateImage(ImageDto dto)
        {
            byte[] contents;
            try
            {
                contents = Convert.FromBase64String(dto.ContentsBase64 ?? "");
            }
            catch (FormatException)
            {
                return BadRequest("Неправильный формат base64");
            }

            var image = new Image(dto.Name ?? "", dto.Width ?? 0, dto.Height ?? 0, dto.Type ?? "", contents);
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Пользователь {User.Identity?.Name} создал картинку с id {image.Id}");
            return image.Id;
        }

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

    internal class ImageInner
    {
        /// <summary>
        /// Идентификатор картинки
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование картинки
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Ширина картинки
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Высота картинки
        /// </summary>
        public decimal Height { get; set; }
        
        /// <summary>
        /// MIME-type картинки
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Содержимое картинки
        /// </summary>
        public byte[] Contents { get; set; } = Array.Empty<byte>();
    }
}
