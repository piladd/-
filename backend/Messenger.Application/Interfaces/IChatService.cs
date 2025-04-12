using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces
{
    public interface IChatService
    {
        Task<Chat> CreateChatAsync(string title);
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<Chat?> GetChatByIdAsync(Guid id);
        Task SendMessageAsync(Guid chatId, Guid senderId, Guid receiverId, string messageText);
        Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId);
    }
}