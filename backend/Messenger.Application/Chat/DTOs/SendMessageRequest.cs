using Messenger.Domain.Enums;

namespace Messenger.Application.Chat.DTOs;

/// <summary>Параметры для отправки зашифрованного сообщения.</summary>
public class SendMessageRequest
{

    /// <summary>Идентификатор получателя.</summary>
    public Guid ReceiverId { get; set; }

    /// <summary>ID чата (обязан быть получен из StartDialog).</summary>
    public Guid ChatId { get; set; }

    /// <summary>Base64-строка с зашифрованным AES-GCM-текстом.</summary>
    public string EncryptedContent { get; set; } = null!;

    /// <summary>Base64-строка с RSA-OAEP-зашифрованным AES-ключом.</summary>
    public string EncryptedAesKey { get; set; } = null!;

    /// <summary>Base64-строка вектора инициализации для AES-GCM.</summary>
    public string Iv { get; set; } = null!;

    /// <summary>Тип сообщения (текст, картинка, файл).</summary>
    public MessageType Type { get; set; } = MessageType.Text;

    public string? Content { get; set; }
}