using Microsoft.EntityFrameworkCore;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Application.Services;

public class ChatService : IChatService
{
    private static readonly List<Chat> MockChats = new()
    {
        new Chat { Id = Guid.NewGuid(), Title = "Общий чат", Messages = new List<Message>() },
        new Chat { Id = Guid.NewGuid(), Title = "Рабочая группа", Messages = new List<Message>() }
    };

    public Task<IEnumerable<Chat>> GetAllChatsAsync()
    {
        return Task.FromResult(MockChats.AsEnumerable());
    }

    public Task<Chat?> GetChatByIdAsync(Guid chatId)
    {
        var chat = MockChats.FirstOrDefault(c => c.Id == chatId);
        return Task.FromResult(chat);
    }

    public Task<Chat> CreateChatAsync(string name)
    {
        var chat = new Chat { Id = Guid.NewGuid(), Title = name, Messages = new List<Message>() };
        MockChats.Add(chat);
        return Task.FromResult(chat);
    }

    public Task SendMessageAsync(Guid chatId, Guid senderId, Guid messageId, string content)
    {
        var chat = MockChats.FirstOrDefault(c => c.Id == chatId);
        if (chat != null)
        {
            chat.Messages.Add(new Message
            {
                Id = messageId,
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.UtcNow
            });
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId)
    {
        var chat = MockChats.FirstOrDefault(c => c.Id == chatId);
        return Task.FromResult(chat?.Messages.AsEnumerable() ?? Enumerable.Empty<Message>());
    }
}