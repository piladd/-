using Messenger.Domain.Enums;

namespace Messenger.Application.Chat.DTOs;

/// <summary>
/// DTO для отправки сообщения. Содержит зашифрованные данные, тип и опциональный ChatId.
/// </summary>
public class SendMessageRequest
{
    public Guid ReceiverId { get; set; }
    public string EncryptedContent { get; set; } = null!;
    public string EncryptedAesKey { get; set; } = null!;
    public string Iv { get; set; } = null!;

    /// <summary>
    /// Идентификатор чата. Необязателен: если не передан, создаётся новый чат.
    /// </summary>
    public Guid? ChatId { get; set; }

    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;

}