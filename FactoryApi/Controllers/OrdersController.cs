using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<OrdersController>  _logger;
        public OrdersController(ApplicationContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /*[HttpPost]
        public async Task<IActionResult> CreateOrder()
        {

        }*/
    }
}
