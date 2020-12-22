using System.ComponentModel.DataAnnotations;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Данные для авторизации
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        /// <example>admin</example>
        [Required]
        public string Login { get; set; } = "";

        /// <summary>
        /// Пароль
        /// </summary>
        /// <example>AAAaaa!2345</example>
        [Required]
        public string Password { get; set; } = "";

        /// <summary>
        /// Запомнить меня
        /// </summary>
        /// <example>true</example>
        public bool RememberMe { get; set; }
    }
}