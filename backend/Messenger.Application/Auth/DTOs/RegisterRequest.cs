namespace Messenger.Application.Auth.DTOs;

/// <summary>
/// Модель запроса регистрации.
/// </summary>
public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}