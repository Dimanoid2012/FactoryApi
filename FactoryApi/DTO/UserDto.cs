namespace FactoryApi.DTO
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Login { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public string? Role { get; set; }
    }
}