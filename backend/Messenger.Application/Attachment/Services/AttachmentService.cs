using System;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.Security;
using Messenger.Infrastructure.Repositories;

namespace Messenger.Application.Services;

/// <summary>
/// Сервис для загрузки и скачивания зашифрованных вложений.
/// </summary>
public class AttachmentService : IAttachmentService
{
    private readonly AttachmentRepository _attachmentRepository;
    private readonly EncryptionService _encryptionService;

    /// <summary>
    /// Конструктор сервиса вложений.
    /// </summary>
    /// <param name="attachmentRepository">Репозиторий вложений</param>
    /// <param name="encryptionService">Сервис шифрования</param>
    public AttachmentService(AttachmentRepository attachmentRepository, EncryptionService encryptionService)
    {
        _attachmentRepository = attachmentRepository;
        _encryptionService = encryptionService;
    }

    /// <summary>
    /// Загружает файл, шифрует его и сохраняет в базу данных.
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <param name="fileData">Массив байт содержимого файла</param>
    /// <returns>ID созданного вложения</returns>
    public async Task<int> UploadAttachmentAsync(string fileName, byte[] fileData)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Имя файла обязательно.", nameof(fileName));

        try
        {
            // Генерируем симметричный ключ для шифрования
            var symKey = _encryptionService.GenerateSymmetricKey();

            // Преобразуем байты файла в строку Base64 (для примера)
            var fileDataBase64 = Convert.ToBase64String(fileData);

            // Шифруем данные с помощью AES
            var (iv, cipherBytes) = _encryptionService.EncryptWithAes(fileDataBase64, symKey);

            // Сохраняем вложение в базу
            var attachment = new Domain.Entities.Attachment
            {
                FileName = fileName,
                EncryptedData = cipherBytes,
                SymmetricKey = Convert.ToBase64String(symKey),
                IV = Convert.ToBase64String(iv),
                CreatedAt = DateTime.UtcNow
            };

            await _attachmentRepository.AddAttachmentAsync(attachment);
            return attachment.Id;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при загрузке вложения.", ex);
        }
    }

    /// <summary>
    /// Получает и расшифровывает вложение по его ID.
    /// </summary>
    /// <param name="attachmentId">ID вложения</param>
    /// <returns>Расшифрованный файл в виде массива байт или null, если не найден</returns>
    public async Task<byte[]?> DownloadAttachmentAsync(int attachmentId)
    {
        try
        {
            var attachment = await _attachmentRepository.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null)
                return null;

            var symKey = Convert.FromBase64String(attachment.SymmetricKey);
            var decryptedString = _encryptionService.DecryptWithAes(
                attachment.EncryptedData,
                symKey,
                Convert.FromBase64String(attachment.IV));

            return Convert.FromBase64String(decryptedString);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ошибка при скачивании вложения.", ex);
        }
    }
}
