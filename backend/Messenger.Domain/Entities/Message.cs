using Messenger.Domain.Enums;

namespace Messenger.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = null!;

        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public MessageType Type { get; set; } = MessageType.Text;
    }
}