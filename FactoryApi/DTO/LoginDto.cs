namespace FactoryApi.DTO
{
    public class LoginDto
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
        /// Запомнить меня
        /// </summary>
        public bool RememberMe { get; set; }
    }
}