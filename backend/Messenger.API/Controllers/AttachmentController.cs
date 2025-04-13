using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using var ms = new MemoryStream();

namespace Messenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

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

    public class UploadAttachmentRequest
    {
        public IFormFile File { get; set; }
    }
}