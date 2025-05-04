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

    /// <summary>
    /// Конструктор сервиса сообщений.
    /// </summary>
    /// <param name="messageRepository">Репозиторий для работы с сущностями Message.</param>
    public MessageService(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    /// <summary>
    /// Отправляет новое сообщение от одного пользователя другому.
    /// </summary>
    /// <param name="senderId">ID отправителя.</param>
    /// <param name="request">Параметры сообщения для отправки.</param>
    /// <returns>DTO отправленного сообщения.</returns>
    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        var message = new Message
        {
            Id               = Guid.NewGuid(),
            ChatId           = request.ChatId,
            SenderId         = senderId,
            ReceiverId       = request.ReceiverId,
            EncryptedContent = request.EncryptedContent,
            EncryptedAesKey  = request.EncryptedAesKey,
            Iv               = request.Iv,
            Content          = request.Content,
            Type             = request.Type,
            SentAt           = DateTime.UtcNow,
            Status           = MessageStatus.Sent
        };

        await _messageRepository.AddAsync(message);

        return new MessageDto
        {
            Id               = message.Id,
            ChatId           = message.ChatId,
            SenderId         = message.SenderId,
            ReceiverId       = message.ReceiverId,
            EncryptedContent = message.EncryptedContent,
            EncryptedAesKey  = message.EncryptedAesKey,
            Iv               = message.Iv,
            SentAt           = message.SentAt,
            Status           = message.Status,
            Type             = message.Type
        };
    }

    /// <summary>
    /// Возвращает историю переписки между двумя пользователями.
    /// </summary>
    /// <param name="senderId">ID первого участника переписки.</param>
    /// <param name="receiverId">ID второго участника переписки.</param>
    /// <returns>Список DTO сообщений, упорядоченный по времени отправки.</returns>
    public async Task<List<MessageDto>> GetChatHistoryAsync(Guid senderId, Guid receiverId)
    {
        var messages = await _messageRepository.GetMessagesBetweenUsersAsync(senderId, receiverId);
        return messages
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id               = m.Id,
                ChatId           = m.ChatId,
                SenderId         = m.SenderId,
                ReceiverId       = m.ReceiverId,
                EncryptedContent = m.EncryptedContent,
                EncryptedAesKey  = m.EncryptedAesKey,
                Iv               = m.Iv,
                SentAt           = m.SentAt,
                Status           = m.Status,
                Type             = m.Type
            })
            .ToList();
    }

    /// <summary>
    /// Обновляет статус сообщения (например, на «Доставлено» или «Прочитано»).
    /// </summary>
    /// <param name="messageId">ID сообщения.</param>
    /// <param name="status">Новый статус сообщения.</param>
    public async Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status)
    {
        await _messageRepository.UpdateStatusAsync(messageId, status);
    }
}