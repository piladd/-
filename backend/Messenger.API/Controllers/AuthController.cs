using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Messenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // POST /login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Имя пользователя и пароль обязательны для ввода.");

            var user = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (user is null)
                return Unauthorized("Неверные учетные данные.");

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponse { Id = user.Id, Username = user.Username, Token = token });
        }

        // POST /register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Имя пользователя и пароль обязательны для ввода.");

            var user = await _authService.RegisterAsync(request.Username, request.Password);
            var token = GenerateJwtToken(user);

            return Ok(new AuthResponse { Id = user.Id, Username = user.Username, Token = token });
        }

        // Генерация JWT токена
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["JWT_KEY"];
            var jwtIssuer = _configuration["JWT_ISSUER"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null, // Можно добавить аудиторий (например, приложение или сервис, который будет потреблять токен)
                claims: claims,
                expires: DateTime.Now.AddHours(1),  // Истечение через 1 час
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO для запроса логина и регистрации
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // DTO для ответа (включает токен)
    public class AuthResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}