using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messenger.Domain.Entities;
using Messenger.Domain.Models;
using Messenger.Security;

namespace Messenger.Application.Services;

public class MessageService
{
    private readonly List<Message> _messages = new(); // In-memory хранилище
    private readonly EncryptionService _encryptionService = new(); // можно внедрить, если нужно

    public Task<Message> SendMessageAsync(SendMessageRequest request)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = Guid.NewGuid(), // если у тебя нет чата — заглушка
            SenderId = Guid.Parse(request.SenderId),
            ReceiverId = Guid.Parse(request.ReceiverId),
            Content = request.Content,
            Timestamp = DateTime.UtcNow
        };

        _messages.Add(message);

        return Task.FromResult(message);
    }

    public IEnumerable<Message> GetMessages()
    {
        return _messages;
    }
}