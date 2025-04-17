namespace Messenger.API.Model;

/// <summary>
/// DTO для загрузки публичного ключа.
/// </summary>
public class UploadKeyDto
{
    /// <summary>
    /// ID пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Публичный ключ пользователя в формате Base64.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;
}