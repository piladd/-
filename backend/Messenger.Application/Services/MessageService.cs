using System;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Models;

namespace Messenger.Application.Services;

public class MessageService : IMessageService
{
    private readonly List<Message> _messages = new();

    public Task<Message> SendMessageAsync(SendMessageRequest request)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = Guid.NewGuid(), // в реальном проекте нужно определять правильный chatId
            SenderId = Guid.Parse(request.SenderId),
            ReceiverId = Guid.Parse(request.ReceiverId),
            Content = request.Content,
            Timestamp = DateTime.UtcNow
        };

        _messages.Add(message);

        return Task.FromResult(message);
    }
}