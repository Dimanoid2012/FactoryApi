using System.ComponentModel.DataAnnotations;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Данные для смены пароля
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Текущий пароль
        /// </summary>
        /// <example>AAAaaa!2345</example>
        [Required]
        public string CurrentPassword { get; set; } = "";

        /// <summary>
        /// Новый пароль
        /// </summary>
        /// <example>AAAaaa!2346</example>
        [Required]
        public string NewPassword { get; set; } = "";

        /// <summary>
        /// Повтор нового пароля
        /// </summary>
        /// <example>AAAaaa!2346</example>
        [Required]
        public string NewPassword2 { get; set; } = "";
    }
}