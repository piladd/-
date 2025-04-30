using System;

namespace Messenger.Application.Chat.DTOs;

public class SendMessageRequest
{
    public Guid ReceiverId { get; set; }
    public string EncryptedContent { get; set; } = default!;
    public string EncryptedAesKey { get; set; } = default!;
    public string Iv { get; set; } = default!;
}