using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.Domain.Entities;
using Messenger.Application.Interfaces;

namespace Messenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Имя пользователя и пароль обязательны для ввода.");

            var user = await _authService.AuthenticateAsync(request.Username, request.Password);
            return user is not null
                ? Ok(new { user.Id, user.Username })
                : Unauthorized("Неверные учетные данные.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Имя пользователя и пароль обязательны для ввода.");

            var user = await _authService.RegisterAsync(request.Username, request.Password);
            return Ok(new { user.Id, user.Username });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}