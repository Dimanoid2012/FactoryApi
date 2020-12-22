using System.ComponentModel.DataAnnotations;

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
        /// <example>reception</example>
        [Required]
        public string Login { get; set; } = "";

        /// <summary>
        /// Пароль
        /// </summary>
        /// <example>AAAaaa!2345</example>
        [Required]
        public string Password { get; set; } = "";

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        /// <example>AAAaaa!2345</example>
        [Required]
        public string Password2 { get; set; } = "";

        /// <summary>
        /// Роль пользователя
        /// </summary>
        /// <example>Reception</example>
        [Required]
        public string Role { get; set; } = "";
    }
}