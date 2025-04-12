using System.Threading.Tasks;
using Messenger.Domain.Models;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface IAuthService
{
    Task<User> AuthenticateAsync(string username, string password);
    Task<User> RegisterAsync(string username, string password);
}