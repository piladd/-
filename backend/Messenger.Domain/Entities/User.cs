namespace Messenger.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;

        // 🔐 Пароль в виде хэша
        public string PasswordHash { get; set; } = string.Empty;

        // навигационные свойства (если есть)
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    }
}