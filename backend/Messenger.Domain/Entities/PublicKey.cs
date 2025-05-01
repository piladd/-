using System;

namespace Messenger.Domain.Entities;

/// <summary>
/// Публичный ключ для шифрования сообщений
/// </summary>
public class PublicKey
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    // Используется при обмене ключами
    public string KeyBase64 { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Для защиты от MITM (опционально)
    public string? Fingerprint { get; set; }  // SHA256 от ключа

    // Подпись ключа, если используется доп. верификация
    public string? Signature { get; set; }
}