using System;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.API.DTOs.Auth;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Messenger.Security;

using Messenger.Application.Services;

namespace Messenger.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserRepository _userRepository;
        private readonly EncryptionService _encryptionService;
        // В реальном решении мастер-ключ должен приходить из защищённой конфигурации
        private readonly string _masterKey;

        public AuthService(UserRepository userRepository, EncryptionService encryptionService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<UserDto> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null || user.Password != password)
                    return null!;

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    PublicKey = user.PublicKey
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка аутентификации.", ex);
            }
        }

        public async Task<UserDto> RegisterAsync(string username, string password)
        {
            try
            {
                var existing = await _userRepository.GetUserByUsernameAsync(username);
                if (existing != null)
                    throw new Exception("Пользователь с таким именем уже существует.");

                var (publicKeyBytes, privateKeyBytes) = _encryptionService.GenerateAsymmetricKeys();
                string publicKey = Convert.ToBase64String(publicKeyBytes);
                // Шифруем приватный ключ с помощью мастер-ключа
                string encryptedPrivateKey = _encryptionService.EncryptPrivateKey(privateKeyBytes, MASTER_KEY);

                var user = new User
                {
                    Username = username,
                    Password = password, // В продакшене храните хэш пароля!
                    PublicKey = publicKey,
                    PrivateKey = encryptedPrivateKey
                };

                await _userRepository.AddUserAsync(user);

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    PublicKey = user.PublicKey
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка регистрации пользователя.", ex);
            }
        }
    }
}
