using System;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chat.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string EncryptedContent { get; set; } = null!;
    public string EncryptedAesKey { get; set; } = null!;
    public string Iv { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public MessageStatus Status { get; set; }
    public MessageType Type { get; set; }

}