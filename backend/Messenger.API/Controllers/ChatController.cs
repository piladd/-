using System.Threading.Tasks;
using Messenger.Domain.Entities;
using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Messenger.Domain.Models;

namespace Messenger.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetChats(string userId)
    {
        try
        {
            var chats = await _chatService.GetAllChatsAsync();
            return Ok(chats);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ошибка при получении списка чатов.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            return chat is not null ? Ok(chat) : NotFound("Чат не найден.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Ошибка при поиске чата.");
        }
    }

    [HttpPost("{chatId}/message")]
    public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MessageText))
            return BadRequest("Текст сообщения обязателен.");

        try
        {
            await _chatService.SendMessageAsync(chatId, request.SenderId, request.ReceiverId, request.MessageText);
            return Ok("Сообщение отправлено.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Ошибка при отправке сообщения.");
        }
    }

    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        try
        {
            var messages = await _chatService.GetMessagesAsync(chatId);
            return Ok(messages);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ошибка при получении сообщений.");
        }
    }
}

public class ChatRequest
{
    public string Title { get; set; } = string.Empty;
}

public class SendMessageRequest
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string MessageText { get; set; } = string.Empty;
}