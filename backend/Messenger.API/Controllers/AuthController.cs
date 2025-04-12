using Messenger.Domain.Entities;
using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.Domain.Models;
using System.Threading.Tasks;

namespace Messenger.API.Controllers;

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
    public async Task<ActionResult<object>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Имя пользователя и пароль обязательны для ввода.");
    
        var user = await _authService.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
            return Unauthorized("Неверные учетные данные.");
    
        return Ok(new { user.Id, user.Username });
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<object>> Register([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Имя пользователя и пароль обязательны для ввода.");
    
        var user = await _authService.RegisterAsync(request.Username, request.Password);
        return Ok(new { user.Id, user.Username, PublicKey = user.PublicKey });
    }
}