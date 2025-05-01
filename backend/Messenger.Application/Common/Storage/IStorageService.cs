namespace Messenger.Application.Common.Storage;

/// <summary>
/// Интерфейс сервиса для хранения и получения файлов (вложения, аватары и т.п.)
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Загружает файл и возвращает его уникальное имя (ключ).
    /// </summary>
    /// <param name="fileName">Оригинальное имя файла</param>
    /// <param name="content">Содержимое файла</param>
    /// <returns>Уникальное имя файла (ключ в хранилище)</returns>
    Task<string> UploadAsync(string fileName, byte[] content);

    /// <summary>
    /// Загружает содержимое файла по имени (ключу).
    /// </summary>
    /// <param name="objectName">Имя/ключ файла</param>
    /// <returns>Содержимое файла или null, если не найден</returns>
    Task<byte[]?> DownloadAsync(string objectName);

    /// <summary>
    /// Удаляет файл по имени.
    /// </summary>
    /// <param name="objectName">Имя/ключ файла</param>
    Task DeleteAsync(string objectName);

    /// <summary>
    /// Проверяет, существует ли файл с указанным именем.
    /// </summary>
    /// <param name="objectName">Имя/ключ файла</param>
    /// <returns>True — если файл существует</returns>
    Task<bool> ExistsAsync(string objectName);
}