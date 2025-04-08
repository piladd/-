using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(Guid chatId, Guid senderId, string content, string type = "Text");
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId);
    }
}