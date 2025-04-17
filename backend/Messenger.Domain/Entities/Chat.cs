namespace Messenger.Domain.Entities;

/// <summary>
/// Сущность чата, содержащая список участников и сообщений.
/// </summary>
public class Chat
{
    /// <summary>
    /// Уникальный идентификатор чата.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название чата.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время создания чата.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Список идентификаторов участников чата.
    /// </summary>
    public List<Guid> ParticipantIds { get; set; } = new();

    /// <summary>
    /// Коллекция сообщений, отправленных в этом чате.
    /// </summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}