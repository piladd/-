using System.Threading.Tasks;
using Messenger.API.DTOs.Auth;

namespace Messenger.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> AuthenticateAsync(string username, string password);
        Task<UserDto> RegisterAsync(string username, string password);
    }
}