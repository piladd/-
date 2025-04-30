using System;

namespace Messenger.Application.Chat.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string EncryptedContent { get; set; } = default!;
    public string EncryptedAesKey { get; set; } = default!;
    public string Iv { get; set; } = default!;
    public DateTime SentAt { get; set; }
}