namespace Messenger.Application.Auth.DTOs;

/// <summary>
/// Ответ после успешной аутентификации.
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = default!;
}