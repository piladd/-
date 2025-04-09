using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace Messenger.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly MessengerDbContext _dbContext;
    private readonly IConfiguration _config;

    public AuthService(MessengerDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    public async Task<User> RegisterAsync(string username, string password)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == username))
            throw new Exception("Пользователь с таким именем уже существует");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return null;

        var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return isValid ? user : null;
    }

    // Дополнительно: генерация JWT
    public string GenerateJwt(User user)
    {
        var jwtKey = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            issuer,
            claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
