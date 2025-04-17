namespace Messenger.Domain.Enums;

/// <summary>
/// Тип сообщения, определяющий его содержимое (текст, изображение, видео и т.д.).
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Текстовое сообщение.
    /// </summary>
    Text,

    /// <summary>
    /// Сообщение с изображением.
    /// </summary>
    Image,

    /// <summary>
    /// Сообщение с видеоконтентом.
    /// </summary>
    Video
}