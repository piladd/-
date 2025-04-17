using System.Threading.Tasks;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с вложениями (файлами).
/// </summary>
public interface IAttachmentService
{
    /// <summary>
    /// Загружает вложение и возвращает его ID.
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <param name="fileData">Массив байт содержимого файла</param>
    /// <returns>ID загруженного вложения</returns>
    Task<int> UploadAttachmentAsync(string fileName, byte[] fileData);

    /// <summary>
    /// Загружает содержимое вложения по его ID.
    /// </summary>
    /// <param name="attachmentId">ID вложения</param>
    /// <returns>Массив байт или null, если не найдено</returns>
    Task<byte[]?> DownloadAttachmentAsync(int attachmentId);
}