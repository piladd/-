using Messenger.Application.Chat.DTOs;
using Messenger.Domain.Entities;
using Messenger.Persistence.Repositories;

namespace Messenger.Application.Chat.Services;

/// <summary>
/// IMessageService для приватного чата 1-на-1.
/// Шифрование/расшифровка — на клиенте, сервер хранит только Base64-строки.
/// </summary>
public class MessageService : IMessageService
{
    private readonly ChatRepository    _chatRepo;
    private readonly MessageRepository _msgRepo;

    public MessageService(
        MessageRepository msgRepo,
        ChatRepository    chatRepo)
    {
        _msgRepo  = msgRepo;
        _chatRepo = chatRepo;
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateDialogAsync(Guid userA, Guid userB)
    {
        // 1) Попробовать найти уже существующий
        var existing = await _chatRepo.GetPrivateChatAsync(userA, userB);
        if (existing != null) return existing.Id;

        // 2) Иначе создаём новый
        var chat = new Domain.Entities.Chat
        {
            Id             = Guid.NewGuid(),
            Title          = $"Dialog_{userA:N}_{userB:N}",
            CreatedAt      = DateTime.UtcNow,
            ParticipantIds = new List<Guid> { userA, userB }
        };
        await _chatRepo.AddAsync(chat);
        return chat.Id;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<MessageDto>> GetDialogHistoryAsync(Guid chatId)
    {
        // получаем все сообщения по chatId
        var msgs = await _chatRepo.GetMessagesByChatIdAsync(chatId);
        return msgs
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
                Type             = m.Type,
                Status           = m.Status,
                SentAt           = m.SentAt
            })
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        // 1) Проверяем, что чат существует (ChatId приходит из контроллера)
        var chat = await _chatRepo.GetByIdAsync(request.ChatId)
                   ?? throw new InvalidOperationException($"Chat {request.ChatId} not found");

        if (!chat.ParticipantIds.Contains(senderId))
            throw new UnauthorizedAccessException("Sender is not a chat participant.");

        // 2) Создаём и сохраняем новое сообщение
        var message = new Message
        {
            Id               = Guid.NewGuid(),
            ChatId           = chat.Id,
            SenderId         = senderId,
            ReceiverId       = request.ReceiverId,
            EncryptedContent = request.EncryptedContent,
            EncryptedAesKey  = request.EncryptedAesKey,
            Iv               = request.Iv,
            Type             = request.Type,
            SentAt           = DateTime.UtcNow,
            Status           = MessageStatus.Sent
        };
        await _msgRepo.AddAsync(message);

        // 3) Возвращаем DTO
        return new MessageDto
        {
            Id               = message.Id,
            ChatId           = message.ChatId,
            SenderId         = message.SenderId,
            ReceiverId       = message.ReceiverId,
            EncryptedContent = message.EncryptedContent,
            EncryptedAesKey  = message.EncryptedAesKey,
            Iv               = message.Iv,
            Type             = message.Type,
            Status           = message.Status,
            SentAt           = message.SentAt
        };
    }

    /// <inheritdoc/>
    public Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status)
        => _msgRepo.UpdateStatusAsync(messageId, status);
}
