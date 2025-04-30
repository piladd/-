using Messenger.Application.Auth.DTOs;

namespace Messenger.Application.Auth.Services;

/// <summary>
/// Сервис аутентификации.
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<string?> LoginAsync(LoginRequest request);
}