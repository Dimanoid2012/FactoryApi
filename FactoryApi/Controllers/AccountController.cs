using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FactoryApi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FactoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController>  _logger;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Создание нового пользователя.
        /// Возможные роли: Administrator, Reception, Writer, Printer, Issuer, Board
        /// </summary>
        /// <response code="204">Пользователь успешно создан. Ничего не возвращает</response>
        /// <response code="400">Ошибка создания пользователя. Возвращает текст ошибки</response>   
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(string))]
        [HttpPost("register")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Password != dto.Password2)
                return BadRequest("Пароли не совпадают");
            if (!new[] {Roles.Administrator, Roles.Reception, Roles.Writer, Roles.Printer, Roles.Issuer, Roles.Board}
                .Contains(dto.Role))
                return BadRequest("Указана несуществующая роль");

            var user = new IdentityUser {UserName = dto.Login};
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    $"Неудачная попытка создания пользователя {dto.Login} пользователем {User.Identity?.Name}: {ErrorsListToString(result.Errors)}");
                return BadRequest($"Ошибка создания нового пользователя: {ErrorsListToString(result.Errors)}");
            }

            result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    $"Неудачная попытка добавления пользователя {dto.Login} к роли {dto.Role} пользователем {User.Identity?.Name}: {ErrorsListToString(result.Errors)}");
                return BadRequest($"Ошибка создания нового пользователя: {ErrorsListToString(result.Errors)}");
            }

            _logger.LogInformation($"Пользователь {dto.Login} успешно зарегистрирован");
            return NoContent();
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <response code="204">Успешная авторизация. Ничего не возвращает</response>
        /// <response code="400">Ошибка авторизации. Возвращает текст ошибки</response>   
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(string))]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Login, login.Password, login.RememberMe, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Неудачная попытка входа пользователя {login.Login}");
                return BadRequest("Не найдена пара пользователь/пароль");
            }

            _logger.LogInformation($"Пользователь {login.Login} зашел в систему");
            return NoContent();
        }
        
        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <response code="204">Успешная смена пароля. Ничего не возвращает</response>
        /// <response code="400">Ошибка смены пароля. Возвращает текст ошибки</response>   
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(string))]
        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            if(passwords.NewPassword != "" && passwords.NewPassword != passwords.NewPassword2)
                return BadRequest("Пароли не совпадают");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest("Ошибка авторизации. Попробуйте заново зайти в систему");
            
            var result = await _userManager.ChangePasswordAsync(user, passwords.CurrentPassword, passwords.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    $"Неудачная попытка смены пароля пользователя {User.Identity?.Name}: {ErrorsListToString(result.Errors)}");
                return BadRequest("Не удалось сменить пароль");
            }

            _logger.LogInformation($"Пользователь {User.Identity?.Name} сменил пароль");
            return NoContent();
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <response code="204">Успешный выход из системы. Ничего не возвращает</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"Пользователь {username} вышел из системы");
            return NoContent();
        }
        
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="username" example="fd058e3f-a5e0-47ef-bf15-3d83edc87a61">Имя пользователя</param>
        /// <response code="204">Пользователь успешно удален. Ничего не возвращает</response>
        /// <response code="400">Ошибка удаления пользователя. Возвращает текст ошибки</response>   
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(string))]
        [HttpDelete("{username}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    $"Неудачная попытка удаления пользователя {username} пользователем {User.Identity?.Name}: {ErrorsListToString(result.Errors)}");
                return BadRequest("Не удалось удалить пользователя");
            }

            _logger.LogInformation($"Пользователь {username} удален из системы");
            return NoContent();
        }

        private static string ErrorsListToString(IEnumerable<IdentityError> errors) =>
            string.Join(',', errors.Select(x => $"{x.Code}: {x.Description}"));
    }
}
