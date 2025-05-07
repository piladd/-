namespace Messenger.Application.Chat.DTOs;

/// <summary>
/// Запрос на создание или получение приватного диалога.
/// </summary>
public class StartDialogRequest
{
    /// <summary>ID собеседника.</summary>
    public Guid InterlocutorId { get; set; }
}