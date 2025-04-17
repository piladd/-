namespace Messenger.API.Model;

/// <summary>
/// Запрос на создание чата.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Название чата.
    /// </summary>
    public string Title { get; set; } = string.Empty;
}