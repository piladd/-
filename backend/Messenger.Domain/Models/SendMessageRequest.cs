namespace Messenger.Domain.Models;

/// <summary>
/// Модель запроса для отправки сообщения между пользователями.
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// ID отправителя сообщения (в виде строки).
    /// </summary>
    public string SenderId { get; set; }

    /// <summary>
    /// ID получателя сообщения (в виде строки).
    /// </summary>
    public string ReceiverId { get; set; }

    /// <summary>
    /// Содержимое сообщения (текст).
    /// </summary>
    public string Content { get; set; }
}