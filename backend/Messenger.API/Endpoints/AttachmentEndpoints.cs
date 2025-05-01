using Microsoft.AspNetCore.Mvc;
using Messenger.Application.Attachment.Interfaces;
using System.Security.Claims;

namespace Messenger.API.Endpoints;

public static class AttachmentEndpoints
{
    public static void MapAttachmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attachments")
            .WithTags("Attachments")
            .RequireAuthorization();

        // Загрузка зашифрованного файла с сохранением метаданных
        group.MapPost("/upload", async (
                    ClaimsPrincipal user,
                    [FromForm] IFormFile file,
                    [FromForm] string encryptedAesKey,
                    [FromForm] string iv,
                    [FromServices] IAttachmentService attachmentService)
                =>
        {
            if (file.Length == 0)
                return Results.BadRequest("Файл не предоставлен.");

            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr is null)
                return Results.Unauthorized();

            var uploaderId = Guid.Parse(userIdStr);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var content = ms.ToArray();

            var objectName = await attachmentService.UploadEncryptedAttachmentAsync(
                file.FileName, content, encryptedAesKey, iv, uploaderId);

            return Results.Ok(new
            {
                objectName,
                encryptedAesKey,
                iv
            });
        });

        // Скачивание файла по objectName
        group.MapGet("/download/{objectName}", async (
                    string objectName,
                    [FromServices] IAttachmentService attachmentService)
                =>
        {
            var content = await attachmentService.DownloadEncryptedAttachmentAsync(objectName);
            if (content == null)
                return Results.NotFound();

            return Results.File(content, "application/octet-stream", objectName);
        });

        // Получение метаданных (ключ + IV) по objectName
        group.MapGet("/meta/{objectName}", async (
                    string objectName,
                    [FromServices] IAttachmentService attachmentService)
                =>
        {
            var meta = await attachmentService.GetAttachmentMetaAsync(objectName);
            return Results.Ok(new
            {
                encryptedAesKey = meta.EncryptedAesKey,
                iv = meta.Iv
            });
        });
    }
}