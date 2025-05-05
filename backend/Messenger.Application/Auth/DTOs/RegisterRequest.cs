namespace Messenger.Application.Auth.DTOs;

/// <summary>
/// Модель запроса регистрации.
/// </summary>
public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    
    /// <summary>
    /// Публичный ключ клиента (SPKI, Base64)
    /// </summary>
    public string PublicKey { get; set; } = null!;
}