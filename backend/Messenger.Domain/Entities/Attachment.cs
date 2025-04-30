using System;

namespace Messenger.Domain.Entities;

/// <summary>
/// Вложение в сообщении или профиль
/// </summary>
public class Attachment
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public byte[] EncryptedData { get; set; } = null!;
    public string SymmetricKey { get; set; } = null!;
    public string IV { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}