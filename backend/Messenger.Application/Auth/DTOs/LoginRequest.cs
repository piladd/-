namespace Messenger.Application.Auth.DTOs;

/// <summary>
/// Модель запроса входа.
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}