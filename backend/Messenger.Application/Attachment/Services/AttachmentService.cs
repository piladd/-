using Messenger.Application.Attachment.Interfaces;
using Messenger.Application.Common.Storage;
using Messenger.Persistence.Repositories;

namespace Messenger.Application.Attachment.Services;

/// <summary>
/// Сервис для приёма и сохранения зашифрованных вложений (E2EE).
/// </summary>
public class AttachmentService : IAttachmentService
{
    private readonly AttachmentRepository _attachmentRepository;
    private readonly IStorageService _storageService;

    /// <summary>
    /// Конструктор сервиса вложений.
    /// </summary>
    public AttachmentService(AttachmentRepository attachmentRepository, IStorageService storageService)
    {
        _attachmentRepository = attachmentRepository;
        _storageService = storageService;
    }

    /// <summary>
    /// Сохраняет зашифрованное вложение (без расшифровки на сервере).
    /// </summary>
    public async Task<string> UploadEncryptedAttachmentAsync(string fileName, byte[] encryptedData, string encryptedAesKey, string iv, Guid uploaderId)
    {
        var objectName = await _storageService.UploadAsync(fileName, encryptedData);

        var attachment = new Domain.Entities.Attachment
        {
            Id = Guid.NewGuid(),
            UploaderId = uploaderId,
            ObjectName = objectName,
            FileName = fileName,
            EncryptedAesKey = encryptedAesKey,
            Iv = iv,
            UploadedAt = DateTime.UtcNow
        };

        await _attachmentRepository.AddAttachmentAsync(attachment);
        return objectName;
    }

    /// <summary>
    /// Возвращает зашифрованный файл по objectName.
    /// </summary>
    public async Task<byte[]?> DownloadEncryptedAttachmentAsync(string objectName)
    {
        return await _storageService.DownloadAsync(objectName);
    }

    /// <summary>
    /// Получает метаинформацию о вложении по objectName (ключ + IV).
    /// </summary>
    public async Task<(string EncryptedAesKey, string Iv)> GetAttachmentMetaAsync(string objectName)
    {
        var meta = await _attachmentRepository.GetMetaAsync(objectName);
        if (meta == null)
            throw new KeyNotFoundException($"Attachment '{objectName}' not found.");

        return meta.Value;
    }
}
