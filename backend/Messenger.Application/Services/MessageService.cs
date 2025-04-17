using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Models;
using Messenger.Security;

namespace Messenger.Application.Services;

/// <summary>
/// Сервис для отправки сообщений (временно работает в памяти).
/// </summary>
public class MessageService : IMessageService
{
    /// <summary>
    /// Внутренний список сообщений, используется как временное хранилище.
    /// </summary>
    private readonly List<Message> _messages = new(); // In-memory хранилище

    /// <summary>
    /// Сервис шифрования. Можно внедрить через DI, если потребуется.
    /// </summary>
    private readonly EncryptionService _encryptionService = new(); // можно внедрить, если нужно

    /// <summary>
    /// Отправляет сообщение, создавая объект Message и добавляя его в память.
    /// </summary>
    /// <param name="request">Запрос, содержащий ID отправителя, получателя и текст</param>
    /// <returns>Созданное сообщение</returns>
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

    /// <summary>
    /// Возвращает все сообщения из внутреннего списка.
    /// </summary>
    /// <returns>Список сообщений</returns>
    public IEnumerable<Message> GetMessages()
    {
        return _messages;
    }
}