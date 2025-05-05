namespace Messenger.Application.Auth.DTOs;

/// <summary>
/// Ответ после успешной аутентификации.
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    
    /// <summary>
    /// Публичный ключ пользователя (SPKI, Base64)
    /// </summary>
    public string PublicKey  { get; set; }  
    
    /// <summary>
    /// Приватный ключ PKCS#8 (Base64). Возвращается только при регистрации.
    /// </summary>
    public string? PrivateKey { get; set; }
}