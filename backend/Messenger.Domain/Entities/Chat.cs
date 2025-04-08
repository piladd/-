namespace Messenger.Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        // Навигационные свойства
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}