using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Messenger.Application.Common.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Endpoints;

public static class AttachmentEndpoints
{
    public static void MapAttachmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attachments")
            .WithTags("Attachments")
            .RequireAuthorization(); // Только авторизованные

        // Загрузка файла
        group.MapPost("/upload", async (
                [FromForm] IFormFile file,
                IStorageService storageService) =>
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

        // Загрузка файла по ключу
        group.MapGet("/download/{objectName}", async (
                string objectName,
                IStorageService storageService) =>
            {
                var content = await storageService.DownloadAsync(objectName);
                if (content == null)
                    return Results.NotFound();

                return Results.File(content, "application/octet-stream", objectName);
            })
            .WithName("DownloadAttachment");
    }
}