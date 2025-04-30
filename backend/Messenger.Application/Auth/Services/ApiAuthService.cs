using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Messenger.Application.Auth.DTOs;
using Messenger.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Application.Auth.Services;

/// <summary>
/// API-сервис для регистрации и логина с генерацией JWT.
/// </summary>
public class ApiAuthService : IAuthService
{
    private readonly AuthService _coreAuthService;
    private readonly IConfiguration _configuration;

    public ApiAuthService(AuthService coreAuthService, IConfiguration configuration)
    {
        _coreAuthService = coreAuthService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var user = await _coreAuthService.RegisterAsync(request.Username, request.Password);
        var token = GenerateJwt(user);
        return new AuthResponse { Token = token };
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var user = await _coreAuthService.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
            return null;

        return GenerateJwt(user);
    }

    private string GenerateJwt(User user)
    {
        var secret = _configuration["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}