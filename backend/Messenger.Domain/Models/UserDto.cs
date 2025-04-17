namespace Messenger.Domain.Models;

/// <summary>
/// DTO (Data Transfer Object) пользователя — используется для передачи основных данных о пользователе.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя пользователя (логин).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Публичный ключ пользователя в формате Base64.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;
}