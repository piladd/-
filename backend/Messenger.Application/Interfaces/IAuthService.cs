using System.Threading.Tasks;
using Messenger.Domain.Models;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса аутентификации и регистрации пользователей.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Пытается авторизовать пользователя по логину и паролю.
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>Пользователь при успешной авторизации, иначе null</returns>
    Task<User> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Регистрирует нового пользователя с логином и паролем.
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>Созданный пользователь с ID и публичным ключом</returns>
    Task<User> RegisterAsync(string username, string password);
}