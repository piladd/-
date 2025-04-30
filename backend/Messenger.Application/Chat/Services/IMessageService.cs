using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messenger.Application.Chat.DTOs;

namespace Messenger.Application.Chat.Services;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request);
    Task<List<MessageDto>> GetChatHistoryAsync(Guid senderId, Guid receiverId);
}