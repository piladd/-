using Messenger.API.Model;
using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.API.Models;

namespace Messenger.API.Controllers;

/// <summary>
/// Контроллер для управления публичными ключами пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class KeysController : ControllerBase
{
    private readonly IKeyStoreService _keyStore;

    /// <summary>
    /// Конструктор контроллера ключей.
    /// </summary>
    /// <param name="keyStore">Сервис для работы с хранилищем ключей</param>
    public KeysController(IKeyStoreService keyStore)
    {
        _keyStore = keyStore;
    }

    /// <summary>
    /// Загружает публичный ключ пользователя.
    /// </summary>
    /// <param name="dto">Объект, содержащий UserId и публичный ключ</param>
    /// <returns>200 OK при успешной загрузке</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] UploadKeyDto dto)
    {
        await _keyStore.SavePublicKeyAsync(dto.UserId, dto.PublicKey);
        return Ok();
    }

    /// <summary>
    /// Получает публичный ключ пользователя по его ID.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Публичный ключ или 404, если не найден</returns>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        var key = await _keyStore.GetPublicKeyAsync(userId);
        if (key == null)
            return NotFound();

        return Ok(new { publicKey = key.KeyBase64 });
    }
}