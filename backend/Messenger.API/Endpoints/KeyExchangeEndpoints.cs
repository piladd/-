using System.Security.Claims;
using Messenger.Application.User.Services;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Endpoints;

public static class KeyExchangeEndpoints
{
    public static void MapKeyExchangeEndpoints(this IEndpointRouteBuilder app)
    {
        // Группа без авторизации, чтобы GET был публичным
        var group = app.MapGroup("/api/keys")
            .WithTags("Key Exchange");

        // POST /api/keys/upload — обновление публичного ключа, требует авторизации
        group.MapPost("/upload", async (
                ClaimsPrincipal user,
                [FromBody] PublicKey key,
                IKeyStoreService keyStore) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                    return Results.Unauthorized();

                key.UserId = Guid.Parse(userId);
                await keyStore.SavePublicKeyAsync(key.UserId, key.KeyBase64);
                return Results.Ok();
            })
            .RequireAuthorization();

        // GET /api/keys/{userId} — получение чужого публичного ключа, публичный доступ
        group.MapGet("/{userId:guid}", async (
                Guid userId,
                IKeyStoreService keyStore) =>
            {
                var key = await keyStore.GetPublicKeyAsync(userId);
                return key is not null
                    ? Results.Ok(key)
                    : Results.NotFound();
            });
    }
}
