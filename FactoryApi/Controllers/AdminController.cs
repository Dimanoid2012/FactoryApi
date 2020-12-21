using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FactoryApi.Models;
using Microsoft.AspNetCore.Authorization;
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
