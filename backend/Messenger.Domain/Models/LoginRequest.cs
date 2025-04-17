namespace Messenger.Domain.Models;

/// <summary>
/// Модель запроса для входа пользователя в систему (логин).
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Имя пользователя (логин).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}