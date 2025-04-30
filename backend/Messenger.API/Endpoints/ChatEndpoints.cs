using System;
using Messenger.Application.Chat.DTOs;
using Messenger.Application.Chat.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Messenger.API.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chat")
            .WithTags("Chat")
            .RequireAuthorization();

        // Отправка сообщения
        group.MapPost("/send", async (
            ClaimsPrincipal user,
            [FromBody] SendMessageRequest request,
            IMessageService messageService) =>
        {
            var senderId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId is null) return Results.Unauthorized();

            var result = await messageService.SendMessageAsync(Guid.Parse(senderId), request);
            return Results.Ok(result);
        });

        // Получение истории чата
        group.MapGet("/history/{recipientId:guid}", async (
            ClaimsPrincipal user,
            Guid recipientId,
            IMessageService messageService) =>
        {
            var senderId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId is null) return Results.Unauthorized();

            var messages = await messageService.GetChatHistoryAsync(Guid.Parse(senderId), recipientId);
            return Results.Ok(messages);
        });
    }
}