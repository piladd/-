namespace Messenger.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public byte[] PublicKey { get; set; } = [];

        // Навигационные свойства
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    }
}