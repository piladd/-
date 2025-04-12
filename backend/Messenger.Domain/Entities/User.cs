namespace Messenger.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        // Для демонстрации пароль хранится в открытом виде – в продакшене используйте хэширование!
        public string Password { get; set; } = string.Empty;
        // Публичный ключ (Base64), который можно отправлять клиенту
        public string PublicKey { get; set; } = string.Empty;
        // Приватный ключ – хранится зашифрованным (см. AuthService)
        public string PrivateKey { get; set; } = string.Empty;
        
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    }
}