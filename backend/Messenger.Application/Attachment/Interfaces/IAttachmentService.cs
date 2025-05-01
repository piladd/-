namespace Messenger.Application.Attachment.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с вложениями (файлами).
/// </summary>
public interface IAttachmentService
{
    /// <summary>
    /// Загружает вложение и возвращает его ID.
    /// </summary>
    /// <returns>ID загруженного вложения</returns>
    Task<string> UploadEncryptedAttachmentAsync(string fileName, byte[] encryptedData, string encryptedAesKey, string iv, Guid uploaderId);


    /// <summary>
    /// Загружает содержимое вложения по его ID.
    /// </summary>
    Task<byte[]?> DownloadEncryptedAttachmentAsync(string objectName);
    
    /// <summary>
    /// Получить мета-информацию вложения по имени объекта
    /// </summary>
    /// <param name="objectName">Имя объекта</param>
    /// <returns>Мета-информация вложения</returns>
    Task<(string EncryptedAesKey, string Iv)> GetAttachmentMetaAsync(string objectName);

}