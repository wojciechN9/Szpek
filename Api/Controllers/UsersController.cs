using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Szpek.Api.Mappings;
using Szpek.Application.User;
using Szpek.Core.Models;
using Szpek.Infrastructure;
using Szpek.Infrastructure.Authorization;
using Szpek.Infrastructure.Email;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public UsersController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        //for now creating only SensorOwners
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreate userCreate)
        {
            var user = new User(userCreate.Username, userCreate.Email);

            if ((await _userManager.FindByEmailAsync(userCreate.Email)) != null)
            {
                return BadRequest("Email exists");
            }

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.SensorOwner);

                if (roleResult.Succeeded)
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _emailSender.SendEmailAsync(
                    user.Email,
                    "Szpek.pl - twój czujnik zanieczyszczenia powietrza",
                    $"<h3>Witaj!</h3><p>Dziękujemy za zakup czujnika. Twój login to: {user.UserName}. Ustaw swoje hasło w serwisie z linku poniżej. Następnie zaloguj się i wejdź w zakładkę 'Instrukcja'</p><a href='https://szpek.pl/passwordChange?username={user.UserName}&token={token}'>Ustaw hasło</a>");

                    return Ok();
                }
                else
                {
                    return BadRequest(roleResult.Errors);
                }
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLogin userLogin)
        {
            var user = await _userManager.FindByNameAsync(userLogin.Username);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userLogin.Username);
            }

            if (user != null)
            {
                var result = await _signInManager
                    .PasswordSignInAsync(user, userLogin.Password, false, false);

                if (!result.Succeeded)
                {
                    return BadRequest("LOGIN_OR_PASSWORD_IS_INCORRECT");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = user.GenerateToken(BackendConfig.JwtSecret, roles);

                var userToken = new UserToken(user.UserName, roles, token);
                return Ok(userToken);
            }
            
            return BadRequest("LOGIN_OR_PASSWORD_IS_INCORRECT");
        }

        [AllowAnonymous]
        [HttpPost("RemindPassword")]
        public async Task<IActionResult> RemindPassword([FromBody]UserRemindPassword userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                return Ok();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Szpek.pl - Przypomnienie hasła",
                $"<h3>Witaj {user.UserName}!</h3><p>Poprosiłeś o zmianę hasła. Kliknij na link poniżej, żeby ustawić nowe</p><a href='https://szpek.pl/passwordChange?username={user.UserName}&token={token}'>Zmień hasło</a>");

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]UserPasswordReset userDto)
        {
            var user = await _userManager.FindByNameAsync(userDto.Username);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ResetPasswordAsync(user, userDto.Token, userDto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
           
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRead>>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users.ToUsersRead());
        }

        [HttpGet("UsersWithoutOwner")]
        public async Task<ActionResult<IEnumerable<UserRead>>> GetUsersWithoutOwner()
        {
            var users = await _userManager.Users
                .Where(u => u.SensorOwnerId == null)
                .ToListAsync();

            return Ok(users.ToUsersRead());
        }
    }
}