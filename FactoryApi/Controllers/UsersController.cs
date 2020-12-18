using System.Collections.Generic;
using System.Threading.Tasks;
using FactoryApi.DTO;
using FactoryApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _db;

        public UsersController(UserRepository db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrator)]
        public Task<IEnumerable<UserDto>> GetUsers() => _db.GetUsers();
    }
}
