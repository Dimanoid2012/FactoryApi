using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FactoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController:ControllerBase
    {
        private readonly ApplicationContext _context;
        
        public TestController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.Select(x => x.UserName).ToListAsync();
            return Ok(users);
        }
    }
}