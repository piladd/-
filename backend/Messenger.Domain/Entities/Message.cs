using System;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Entities;

/// <summary>
/// Сообщение в чате
/// </summary>
public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public string EncryptedContent { get; set; } = null!;
    public string EncryptedAesKey { get; set; } = null!;
    public string Iv { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}