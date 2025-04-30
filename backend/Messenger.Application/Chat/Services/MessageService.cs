using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messenger.Application.Chat.DTOs;
using Messenger.Domain.Entities;
using Messenger.Persistence.Repositories;

namespace Messenger.Application.Chat.Services;

/// <summary>
/// Сервис для отправки и получения сообщений между пользователями.
/// </summary>
public class MessageService : IMessageService
{
    private readonly MessageRepository _messageRepository;

    public MessageService(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            EncryptedContent = request.EncryptedContent,
            EncryptedAesKey = request.EncryptedAesKey,
            Iv = request.Iv,
            SentAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message);

        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            EncryptedContent = message.EncryptedContent,
            EncryptedAesKey = message.EncryptedAesKey,
            Iv = message.Iv,
            SentAt = message.SentAt
        };
    }

    public async Task<List<MessageDto>> GetChatHistoryAsync(Guid senderId, Guid receiverId)
    {
        var messages = await _messageRepository.GetMessagesBetweenUsersAsync(senderId, receiverId);

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            EncryptedContent = m.EncryptedContent,
            EncryptedAesKey = m.EncryptedAesKey,
            Iv = m.Iv,
            SentAt = m.SentAt
        }).ToList();
    }
}