using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using FactoryApi.DTO;
using Npgsql;

namespace FactoryApi.Repositories
{
    public class UserRepository
    {
        private readonly string _connectinString;
        public UserRepository(string connectionString) => _connectinString = connectionString;

        /// <summary>
        /// Возвращает всех пользователей с их ролями
        /// </summary>
        /// <returns>Последовательность объектов UserDto</returns>
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            await using var db = new NpgsqlConnection(_connectinString);
            return await db.QueryAsync<UserDto>(@"
                SELECT u.""UserName"" ""Login"", r.""Name"" ""Role"" 
                FROM ""AspNetUsers"" u
                LEFT JOIN ""AspNetUserRoles"" ur ON ur.""UserId"" = u.""Id""
                LEFT JOIN ""AspNetRoles"" r ON r.""Id"" = ur.""RoleId""");
        }
    }
}