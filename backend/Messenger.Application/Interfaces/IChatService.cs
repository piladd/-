using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messenger.Application.Interfaces;

public interface IChatService
{
    Task<IEnumerable<Chat>> GetAllChatsAsync();
    Task<Chat?> GetChatByIdAsync(Guid chatId);
    Task<Chat> CreateChatAsync(string name);
    Task SendMessageAsync(Guid chatId, Guid senderId, Guid messageId, string content);
    Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId);
}