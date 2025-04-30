using System;

namespace Messenger.Domain.Entities;

/// <summary>
/// Публичный ключ для шифрования сообщений
/// </summary>
public class PublicKey
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string KeyBase64 { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}