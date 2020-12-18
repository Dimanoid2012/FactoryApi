using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using FactoryApi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString;

        public UsersController(ConnectionString connectionString)
        {
            _connectionString = connectionString.Value;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            await using var db = new NpgsqlConnection(_connectionString);
            return await db.QueryAsync<UserDto>(@"
                SELECT u.""UserName"" ""Login"", r.""Name"" ""Role"" 
                FROM ""AspNetUsers"" u
                LEFT JOIN ""AspNetUserRoles"" ur ON ur.""UserId"" = u.""Id""
                LEFT JOIN ""AspNetRoles"" r ON r.""Id"" = ur.""RoleId""");
        }
    }
}
