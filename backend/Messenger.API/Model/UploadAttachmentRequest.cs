namespace Messenger.API.Model;

/// <summary>
/// Модель запроса на загрузку вложения.
/// </summary>
public class UploadAttachmentRequest
{
    /// <summary>
    /// Загружаемый файл.
    /// </summary>
    public IFormFile File { get; set; } = default!;
}