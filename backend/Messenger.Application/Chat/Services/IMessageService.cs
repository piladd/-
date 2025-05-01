using Messenger.Application.Chat.DTOs;
using Messenger.Domain.Entities;

namespace Messenger.Application.Chat.Services;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request);
    Task<List<MessageDto>> GetChatHistoryAsync(Guid senderId, Guid receiverId);

    Task UpdateMessageStatusAsync(Guid messageId, MessageStatus delivered);
}