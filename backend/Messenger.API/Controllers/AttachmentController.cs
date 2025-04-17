using Messenger.API.Model;
using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

/// <summary>
/// Контроллер для загрузки и скачивания вложений.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    /// <summary>
    /// Конструктор контроллера, получает сервис вложений через DI.
    /// </summary>
    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    /// <summary>
    /// Загружает файл на сервер.
    /// </summary>
    /// <param name="request">Форма с файлом (multipart/form-data)</param>
    /// <returns>ID вложения или сообщение об ошибке</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadAttachmentRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("Файл не выбран.");

        try
        {
            using var ms = new MemoryStream();
            await request.File.CopyToAsync(ms);
            var fileData = ms.ToArray();

            var attachmentId = await _attachmentService.UploadAttachmentAsync(request.File.FileName, fileData);
            return Ok(new { AttachmentId = attachmentId });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ошибка при загрузке файла.");
        }
    }

    /// <summary>
    /// Скачивает вложение по его ID.
    /// </summary>
    /// <param name="id">ID вложения</param>
    /// <returns>Файл или сообщение об ошибке</returns>
    [HttpGet("download/{id}")]
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var fileBytes = await _attachmentService.DownloadAttachmentAsync(id);
            if (fileBytes == null)
                return NotFound("Вложение не найдено.");

            return File(fileBytes, "application/octet-stream", $"attachment_{id}.dat");
        }
        catch (Exception)
        {
            return StatusCode(500, "Ошибка при скачивании файла.");
        }
    }
}