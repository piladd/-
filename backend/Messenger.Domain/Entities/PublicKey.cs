namespace Messenger.Domain.Entities;

/// <summary>
/// Сущность публичного ключа, связанного с пользователем.
/// </summary>
public class PublicKey
{
    /// <summary>
    /// Уникальный идентификатор записи ключа.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Идентификатор пользователя, которому принадлежит ключ.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Публичный ключ в формате Base64.
    /// </summary>
    public string KeyBase64 { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время загрузки ключа.
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}