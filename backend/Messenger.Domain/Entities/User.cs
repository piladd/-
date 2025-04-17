namespace Messenger.Domain.Entities;

/// <summary>
/// Сущность пользователя мессенджера.
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя пользователя (логин).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя (в демонстрационных целях хранится в открытом виде).
    /// В реальном приложении необходимо использовать хэширование!
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Публичный ключ пользователя в формате Base64. Отправляется другим участникам.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Приватный ключ пользователя, зашифрованный при сохранении.
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// Коллекция сообщений, отправленных этим пользователем.
    /// </summary>
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
}