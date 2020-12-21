using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FactoryApi.DTO;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("register")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Login == null)
                return BadRequest("Не указано имя пользователя");
            if (dto.Password == null)
                return BadRequest("Не указан пароль");
            if (dto.Password2 == null)
                return BadRequest("Не указано подтверждение пароля");
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (login.Login == null)
                return BadRequest("Не указано имя пользователя");
            if (login.Password == null)
                return BadRequest("Не указан пароль");
            
            var result = await _signInManager.PasswordSignInAsync(login.Login, login.Password, login.RememberMe, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Неудачная попытка входа пользователя {login.Login}");
                return BadRequest("Не найдена пара пользователь/пароль");
            }

            _logger.LogInformation($"Пользователь {login.Login} зашел в систему");
            return NoContent();
        }
        
        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            if (passwords.CurrentPassword == null)
                return BadRequest("Не указан текущий пароль");
            if (passwords.NewPassword == null)
                return BadRequest("Не указан новый пароль");
            if (passwords.NewPassword2 == null)
                return BadRequest("Не указано подтверждение пароля");
            if(passwords.NewPassword != passwords.NewPassword2)
                return BadRequest("Пароли не совпадают");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest("Ошибка авторизации. Попробуйте заново зайти в систему");
            
            var result = await _userManager.ChangePasswordAsync(user, passwords.CurrentPassword, passwords.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    $"Неудачная попытка смены пароля пользователя {User.Identity?.Name}: {result.Errors}");
                return BadRequest("Не удалось сменить пароль");
            }

            _logger.LogInformation($"Пользователь {User.Identity?.Name} сменил пароль");
            return NoContent();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"Пользователь {username} вышел из системы");
            return NoContent();
        }

        private static string ErrorsListToString(IEnumerable<IdentityError> errors) =>
            string.Join(',', errors.Select(x => $"{x.Code}: {x.Description}"));
    }
}
