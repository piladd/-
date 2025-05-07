using System.Security.Claims;
using Messenger.Application.Chat.DTOs;
using Messenger.Application.Chat.Services;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chat")
            .WithTags("Chat")
            .RequireAuthorization();

        // Отправка сообщения (ChatId передаётся в теле запроса)
        group.MapPost("/send", async (
                ClaimsPrincipal user,
                [FromBody] SendMessageRequest request,
                [FromServices] IMessageService messageService) =>
            {
                var senderIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (senderIdStr is null)
                    return Results.Unauthorized();

                var senderId = Guid.Parse(senderIdStr);
                var result   = await messageService.SendMessageAsync(senderId, request);

                return result is null
                    ? Results.BadRequest("Чат с получателем не найден.")
                    : Results.Ok(result);
            })
            .WithName("SendMessage")
            .Accepts<SendMessageRequest>("application/json")
            .Produces<MessageDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        // Получение истории диалога по chatId
        group.MapGet("/history/{chatId:guid}", async (
                ClaimsPrincipal user,
                Guid chatId,
                [FromServices] IMessageService messageService) =>
            {
                var senderIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (senderIdStr is null)
                    return Results.Unauthorized();

                // Получаем зашифрованную историю по одному параметру chatId
                var history = await messageService.GetDialogHistoryAsync(chatId);
                return Results.Ok(history);
            })
            .WithName("GetChatHistory")
            .Produces<IEnumerable<MessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/start", async (
                ClaimsPrincipal user,
                [FromBody] StartDialogRequest req,
                IMessageService svc) =>
            {
                var me = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (me == null) return Results.Unauthorized();

                var chatId = await svc.CreateDialogAsync(Guid.Parse(me), req.InterlocutorId);
                return Results.Ok(new { chatId });
            })
            .WithName("StartDialog")
            .Accepts<StartDialogRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    public static void MapMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/messages")
            .WithTags("Messages")
            .RequireAuthorization();

        // Пометить доставленным
        group.MapPost("/delivered/{messageId:guid}", async (
                Guid messageId,
                [FromServices] IMessageService messageService,
                ClaimsPrincipal user) =>
            {
                if (user.FindFirstValue(ClaimTypes.NameIdentifier) is null)
                    return Results.Unauthorized();

                await messageService.UpdateMessageStatusAsync(messageId, MessageStatus.Delivered);
                return Results.Ok();
            })
            .WithName("MarkDelivered")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // Пометить прочитанным
        group.MapPost("/read/{messageId:guid}", async (
                Guid messageId,
                [FromServices] IMessageService messageService,
                ClaimsPrincipal user) =>
            {
                if (user.FindFirstValue(ClaimTypes.NameIdentifier) is null)
                    return Results.Unauthorized();

                await messageService.UpdateMessageStatusAsync(messageId, MessageStatus.Read);
                return Results.Ok();
            })
            .WithName("MarkRead")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}