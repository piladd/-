using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.Domain.Models;

namespace Messenger.API.Controllers;

/// <summary>
/// Контроллер для авторизации и регистрации пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Конструктор контроллера авторизации.
    /// </summary>
    /// <param name="authService">Сервис авторизации</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Авторизует пользователя по имени и паролю.
    /// </summary>
    /// <param name="request">Данные для входа: имя пользователя и пароль</param>
    /// <returns>Идентификатор и имя пользователя при успехе, 401 если данные неверны</returns>
    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Имя пользователя и пароль обязательны для ввода.");

        var user = await _authService.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
            return Unauthorized("Неверные учетные данные.");

        return Ok(new { user.Id, user.Username });
    }

    /// <summary>
    /// Регистрирует нового пользователя с заданными логином и паролем.
    /// </summary>
    /// <param name="request">Данные для регистрации</param>
    /// <returns>ID, имя и публичный ключ нового пользователя</returns>
    [HttpPost("register")]
    public async Task<ActionResult<object>> Register([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Имя пользователя и пароль обязательны для ввода.");

        var user = await _authService.RegisterAsync(request.Username, request.Password);
        return Ok(new { user.Id, user.Username, PublicKey = user.PublicKey });
    }
}