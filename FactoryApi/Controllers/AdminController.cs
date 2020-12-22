using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
    public class AdminController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly Configuration _configuration;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationContext context, Configuration configuration,
            ILogger<AdminController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        
        /// <summary>
        /// Перезапускает сессию: удаляет все заказы, включает или выключает этап нанесения
        /// </summary>
        /// <param name="enableWriting" example="true">Включить этап нанесения</param>
        /// <response code="204">Успешный перезапуск сессии. Все заказы удалены. Ничего не возвращает</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("resetSession")]
        public async Task<IActionResult> ResetSession(bool enableWriting)
        {
            var orders = await _context.Orders.ToListAsync();

            _context.Orders.RemoveRange(orders);
            
            await _context.SaveChangesAsync();

            _configuration.EnableWriting = enableWriting;

            _logger.LogInformation($"Пользователь {User.Identity?.Name} удалил все заказы");
            return NoContent();
        }
        
        /// <summary>
        /// Возвращает логи сервера в виде текста
        /// </summary>
        /// <response code="200">Возвращает логи в виде текста</response>
        /// <response code="400">Ошибка чтения логов. Возвращает текст ошибки</response>
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                var sb = new StringBuilder();
                var dir = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                        ?.Substring(10) ?? "", "logs");
                foreach (var file in Directory.GetFiles(dir, "*all*.log"))
                {
                    sb.Append(await System.IO.File.ReadAllTextAsync(file));
                }

                return Ok(sb.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
