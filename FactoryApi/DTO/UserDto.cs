using System.ComponentModel.DataAnnotations;

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
        /// <example>reception</example>
        [Required]
        public string Login { get; set; } = "";

        /// <summary>
        /// Роль
        /// </summary>
        /// <example>Reception</example>
        [Required]
        public string Role { get; set; } = "";
    }
}