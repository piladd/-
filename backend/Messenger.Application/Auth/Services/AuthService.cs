using System;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Messenger.Security;
using Microsoft.Extensions.Configuration;

namespace Messenger.Application.Services;

/// <summary>
/// Сервис авторизации и регистрации пользователей с генерацией ключей и шифрованием.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserRepository _userRepository;
    private readonly EncryptionService _encryptionService;
    private readonly string _masterKey;

    /// <summary>
    /// Конструктор сервиса, получает зависимости и мастер-ключ из конфигурации.
    /// </summary>
    public AuthService(UserRepository userRepository, EncryptionService encryptionService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _masterKey = configuration["Security:MasterKey"] ?? throw new ArgumentNullException("MasterKey not found in configuration.");
    }

    /// <summary>
    /// Проверяет логин и пароль пользователя и возвращает объект пользователя при успешной аутентификации.
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>Пользователь или null, если данные неверны</returns>
    public async Task<User> AuthenticateAsync(string username, string password)
    {
        try
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || user.Password != password)
                return null!;

            return user;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка аутентификации.", ex);
        }
    }

    /// <summary>
    /// Регистрирует нового пользователя, генерирует асимметричные ключи и шифрует приватный ключ.
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>Созданный пользователь</returns>
    public async Task<User> RegisterAsync(string username, string password)
    {
        try
        {
            var existing = await _userRepository.GetUserByUsernameAsync(username);
            if (existing != null)
                throw new Exception("Пользователь с таким именем уже существует.");

            // Генерация пары ключей
            var (publicKeyBytes, privateKeyBytes) = _encryptionService.GenerateAsymmetricKeys();

            // Сохранение публичного и зашифрованного приватного ключа
            string publicKey = Convert.ToBase64String(publicKeyBytes);
            string encryptedPrivateKey = _encryptionService.EncryptPrivateKey(privateKeyBytes, _masterKey);

            var user = new User
            {
                Username = username,
                Password = password,
                PublicKey = publicKey,
                PrivateKey = encryptedPrivateKey
            };

            await _userRepository.AddUserAsync(user);

            return user;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка регистрации пользователя.", ex);
        }
    }
}
