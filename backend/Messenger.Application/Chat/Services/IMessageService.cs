using Messenger.Application.Chat.DTOs;
using Messenger.Domain.Entities;

namespace Messenger.Application.Chat.Services;

/// <summary>
/// Сервис приватных диалогов 1-на-1 и обмена зашифрованными сообщениями.
/// </summary>
public interface IMessageService
{
    /// <summary>Создаёт или возвращает уже существующий приватный чат между двумя пользователями.</summary>
    Task<Guid> CreateDialogAsync(Guid userA, Guid userB);

    /// <summary>Возвращает всю историю диалога (шифротекст).</summary>
    Task<IReadOnlyList<MessageDto>> GetDialogHistoryAsync(Guid chatId);

    /// <summary>Сохраняет одно зашифрованное сообщение.</summary>
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request);

    /// <summary>Обновляет статус доставки или прочтения сообщения.</summary>
    Task UpdateMessageStatusAsync(Guid messageId, MessageStatus status);
}