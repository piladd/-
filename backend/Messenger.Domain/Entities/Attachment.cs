namespace Messenger.Domain.Entities;

/// <summary>
/// Сущность вложения (файла), хранящегося в зашифрованном виде.
/// </summary>
public class Attachment
{
    /// <summary>
    /// Уникальный идентификатор вложения.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя файла (оригинальное).
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Зашифрованное содержимое файла (Base64 в виде массива байтов).
    /// </summary>
    public byte[] EncryptedData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Симметричный ключ, зашифрованный и закодированный в Base64.
    /// </summary>
    public string SymmetricKey { get; set; } = string.Empty;

    /// <summary>
    /// Вектор инициализации (IV) для шифрования AES, закодированный в Base64.
    /// </summary>
    public string IV { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время загрузки вложения.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}