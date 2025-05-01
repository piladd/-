using System;

namespace Messenger.Domain.Entities;

/// <summary>
/// Вложение, загружаемое пользователем (E2EE).
/// </summary>
public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UploaderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public string EncryptedAesKey { get; set; } = string.Empty;
    public string Iv { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}