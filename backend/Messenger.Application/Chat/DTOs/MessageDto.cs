using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chat.DTOs;

/// <summary>
/// Возвращается сервером – хранит только шифротекст.
/// </summary>
public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    
    public string EncryptedContent { get; set; } = null!;
    public string EncryptedAesKey  { get; set; } = null!;
    public string Iv               { get; set; } = null!;
    public MessageType Type        { get; set; }
    public MessageStatus Status    { get; set; }
    public DateTime SentAt         { get; set; }
}