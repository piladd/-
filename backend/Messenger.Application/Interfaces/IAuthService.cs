using System.Threading.Tasks;
using Messenger.Application.Auth.DTOs;

namespace Messenger.Application.Auth.Services;

/// <summary>
/// Сервис аутентификации.
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);

}