using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces
{
    public interface IChatService
    {
        Task<Chat> CreateChatAsync(string title);
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<Chat?> GetChatByIdAsync(Guid id);
    }
}