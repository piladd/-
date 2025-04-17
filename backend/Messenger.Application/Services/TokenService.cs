using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Messenger.Application.Services;

/// <summary>
/// Сервис для генерации JWT-токенов аутентификации.
/// </summary>
public class TokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор сервиса токенов, получает доступ к конфигурации.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения</param>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Генерирует JWT-токен с данными пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Строка JWT-токена</returns>
    public string GenerateToken(string userId, string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}