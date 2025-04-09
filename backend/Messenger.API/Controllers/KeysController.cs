using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeysController : ControllerBase
{
    private readonly IKeyStoreService _keyStore;

    public KeysController(IKeyStoreService keyStore)
    {
        _keyStore = keyStore;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] UploadKeyDto dto)
    {
        await _keyStore.SavePublicKeyAsync(dto.UserId, dto.PublicKey);
        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        var key = await _keyStore.GetPublicKeyAsync(userId);
        if (key == null)
            return NotFound();

        return Ok(new { publicKey = key.KeyBase64 });
    }
}

public class UploadKeyDto
{
    public Guid UserId { get; set; }
    public string PublicKey { get; set; } = string.Empty;
}