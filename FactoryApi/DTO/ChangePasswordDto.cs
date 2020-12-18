namespace FactoryApi.DTO
{
    public class ChangePasswordDto
    {
        /// <summary>
        /// Текущий пароль
        /// </summary>
        public string? CurrentPassword { get; set; }
        
        /// <summary>
        /// Новый пароль
        /// </summary>
        public string? NewPassword { get; set; }
        
        /// <summary>
        /// Повтор нового пароля
        /// </summary>
        public string? NewPassword2 { get; set; }
    }
}