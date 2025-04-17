using System;

namespace Messenger.API.Models;

/// <summary>
/// Запрос на отправку сообщения.
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// ID отправителя.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// ID получателя.
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// Текст сообщения.
    /// </summary>
    public string MessageText { get; set; } = string.Empty;
}