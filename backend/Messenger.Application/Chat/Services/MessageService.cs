using Messenger.Application.Chat.DTOs;
using Messenger.Domain.Entities;
using Messenger.Persistence.Repositories;

namespace Messenger.Application.Chat.Services;

/// <summary>
/// Сервис для отправки и получения сообщений между пользователями.
/// </summary>
public class MessageService(MessageRepository messageRepository) : IMessageService
{
    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            EncryptedContent = request.EncryptedContent,
            EncryptedAesKey = request.EncryptedAesKey,
            Iv = request.Iv,
            SentAt = DateTime.UtcNow,
            Content = request.Content,
            Type = request.Type
        };

        await messageRepository.AddAsync(message);

        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            EncryptedContent = message.EncryptedContent,
            EncryptedAesKey = message.EncryptedAesKey,
            Iv = message.Iv,
            SentAt = message.SentAt,
            Type = message.Type
        };
    }

    public async Task<List<MessageDto>> GetChatHistoryAsync(Guid senderId, Guid receiverId)
    {
        var messages = await messageRepository.GetMessagesBetweenUsersAsync(senderId, receiverId);

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            EncryptedContent = m.EncryptedContent,
            EncryptedAesKey = m.EncryptedAesKey,
            Iv = m.Iv,
            SentAt = m.SentAt,
            Status = m.Status
        }).ToList();
    }
    
    public async Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status)
    {
        await messageRepository.UpdateStatusAsync(messageId, status);
    }

}