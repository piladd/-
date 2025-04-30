namespace Messenger.Application.Auth.DTOs;

/// <summary>
/// Модель запроса входа.
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}