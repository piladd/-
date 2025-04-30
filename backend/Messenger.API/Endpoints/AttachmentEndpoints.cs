using Microsoft.AspNetCore.Mvc;
using Messenger.Application.Common.Storage;

namespace Messenger.API.Endpoints;

public static class AttachmentEndpoints
{
    public static void MapAttachmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attachments")
            .WithTags("Attachments")
            .RequireAuthorization();

        // Загрузка файла
        group.MapPost("/upload", async (
                    [FromForm] IFormFile file,
                    [FromServices] IStorageService storageService)
                =>
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest("Файл не предоставлен.");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var content = ms.ToArray();

                var objectName = await storageService.UploadAsync(file.FileName, content);
                return Results.Ok(new { objectName });
            })
            .WithName("UploadAttachment");

        // Скачивание файла
        group.MapGet("/download/{objectName}", async (
                    string objectName,
                    [FromServices] IStorageService storageService)
                =>
            {
                var content = await storageService.DownloadAsync(objectName);
                if (content == null)
                    return Results.NotFound();

                return Results.File(content, "application/octet-stream", objectName);
            })
            .WithName("DownloadAttachment");
    }
}