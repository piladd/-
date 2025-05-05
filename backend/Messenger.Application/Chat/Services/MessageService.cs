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
    private readonly ChatRepository _chatRepository;

    public MessageService(
        MessageRepository messageRepository,
        ChatRepository chatRepository)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
    }

    /// <summary>
    /// Отправляет новое сообщение от одного пользователя другому.
    /// Создаёт чат, если он ещё не существует.
    /// </summary>
    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        Guid chatId;

        if (request.ChatId.HasValue && request.ChatId.Value != Guid.Empty)
        {
            var existing = await _chatRepository.GetByIdAsync(request.ChatId.Value);
            if (existing is null)
                throw new Exception("Чат с указанным ID не существует.");

            chatId = existing.Id;
        }
        else
        {
            var privateChat = await _chatRepository.GetPrivateChatAsync(senderId, request.ReceiverId);

            if (privateChat is null)
            {
                privateChat = new Domain.Entities.Chat
                {
                    Id = Guid.NewGuid(),
                    Title = $"Chat_{senderId}_{request.ReceiverId}",
                    CreatedAt = DateTime.UtcNow,
                    ParticipantIds = [senderId, request.ReceiverId],
                    Messages = new List<Message>()
                };
                await _chatRepository.AddAsync(privateChat);
            }

            chatId = privateChat.Id;
        }

        // Создаём сообщение
        var message = new Message
        {
            Id               = Guid.NewGuid(),
            ChatId           = chatId,
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

    public async Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status)
    {
        await _messageRepository.UpdateStatusAsync(messageId, status);
    }
}
