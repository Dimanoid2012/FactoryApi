namespace FactoryApi.DTO
{
    /// <summary>
    /// Данные регистрации нового пользователя
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        public string? Password2 { get; set; }
        
        /// <summary>
        /// Роль пользователя
        /// </summary>
        public string? Role { get; set; }
    }
}