namespace Messenger.Domain.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}