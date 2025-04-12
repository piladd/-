using Messenger.Domain.Enums;
using System;

namespace Messenger.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;
        
        // Зашифрованное содержание (AES‑ciphertext, Base64)
        public string Content { get; set; } = string.Empty;
        // Зашифрованный симметричный ключ (RSA‑ciphertext, Base64)
        public string EncryptedKey { get; set; } = string.Empty;
        // IV для AES (Base64)
        public string IV { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
    }
}