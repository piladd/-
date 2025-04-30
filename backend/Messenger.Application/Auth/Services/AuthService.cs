using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Messenger.Application.Auth.DTOs;
using Messenger.Domain.Entities;
using Messenger.Persistence.Repositories;
using Messenger.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Application.Auth.Services
{
    /// <summary>
    /// Сервис авторизации и регистрации пользователей с генерацией JWT и шифрованием ключей.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserRepository _userRepository;
        private readonly EncryptionService _encryptionService;
        private readonly IConfiguration _configuration;
        private readonly string _masterKey;

        public AuthService(
            UserRepository userRepository,
            EncryptionService encryptionService,
            IConfiguration configuration)
        {
            _userRepository    = userRepository;
            _encryptionService = encryptionService;
            _configuration     = configuration;
            _masterKey         = configuration["Security:MasterKey"]
                                 ?? throw new InvalidOperationException("MasterKey not found in configuration.");
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Проверяем, что пользователя с таким именем ещё нет
            if (await _userRepository.GetByUsernameAsync(request.Username) is not null)
                throw new ApplicationException("Пользователь с таким именем уже существует.");

            // Генерируем ключи и шифруем приватный
            var (pubBytes, privBytes) = _encryptionService.GenerateAsymmetricKeys();
            var publicKey            = Convert.ToBase64String(pubBytes);
            var encryptedPrivateKey  = _encryptionService.EncryptPrivateKey(privBytes, _masterKey);

            // Создаём и сохраняем сущность User
            var user = new Domain.Entities.User {
                Username   = request.Username,
                Password   = request.Password,
                PublicKey  = publicKey,
                PrivateKey = encryptedPrivateKey
            };
            await _userRepository.AddUserAsync(user);

            // Формируем ответ с токеном, ID и именем
            return new AuthResponse {
                Token    = GenerateJwt(user),
                UserId   = user.Id,
                Username = user.Username
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || user.Password != request.Password)
                return null;

            return new AuthResponse {
                Token    = GenerateJwt(user),
                UserId   = user.Id,
                Username = user.Username
            };
        }

        private string GenerateJwt(Domain.Entities.User user)
        {
            var secret      = _configuration["Jwt:Secret"]
                              ?? throw new InvalidOperationException("JWT secret is not configured.");
            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds       = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresHour = int.TryParse(_configuration["Jwt:ExpiresHours"], out var h) ? h : 12;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.Username)
            };

            var token = new JwtSecurityToken(
                issuer:             _configuration["Jwt:Issuer"],
                audience:           _configuration["Jwt:Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(expiresHour),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
