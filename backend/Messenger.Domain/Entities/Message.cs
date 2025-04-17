using Messenger.Domain.Enums;

namespace Messenger.Domain.Entities;

/// <summary>
/// Сущность сообщения, отправляемого между пользователями в чате.
/// </summary>
public class Message
{
    /// <summary>
    /// Конструктор, принимающий получателя.
    /// </summary>
    public Message(User receiver)
    {
        Receiver = receiver;
    }

    /// <summary>
    /// Пустой конструктор.
    /// </summary>
    public Message() { }

    /// <summary>
    /// Уникальный идентификатор сообщения.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор чата, к которому принадлежит сообщение.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Навигационное свойство — чат, в котором отправлено сообщение.
    /// </summary>
    public Chat Chat { get; set; } = null!;

    /// <summary>
    /// Идентификатор отправителя сообщения.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Навигационное свойство — пользователь, который отправил сообщение.
    /// </summary>
    public User Sender { get; set; } = null!;

    /// <summary>
    /// Идентификатор получателя сообщения.
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// Навигационное свойство — пользователь, который получает сообщение.
    /// </summary>
    public User Receiver { get; set; }

    /// <summary>
    /// Зашифрованное содержимое сообщения (AES, Base64).
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Зашифрованный симметричный ключ (RSA, Base64).
    /// </summary>
    public string EncryptedKey { get; set; } = string.Empty;

    /// <summary>
    /// Вектор инициализации для AES (Base64).
    /// </summary>
    public string IV { get; set; } = string.Empty;

    /// <summary>
    /// Время отправки сообщения (локальное/отправитель).
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Тип сообщения (текст, файл и т.д.).
    /// </summary>
    public MessageType Type { get; set; } = MessageType.Text;

    /// <summary>
    /// Момент создания или записи сообщения в систему.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
