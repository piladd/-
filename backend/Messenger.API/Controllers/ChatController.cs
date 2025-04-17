using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Messenger.API.Models;

namespace Messenger.API.Controllers;

/// <summary>
/// Контроллер для управления чатами и сообщениями.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    /// <summary>
    /// Конструктор контроллера чатов.
    /// </summary>
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Получает список чатов, в которых участвует пользователь.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список чатов</returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetChats(string userId)
    {
        try
        {
            if (!Guid.TryParse(userId, out var guid))
                return BadRequest("Некорректный userId");

            var chats = await _chatService.GetAllChatsAsync(guid);
            return Ok(chats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при получении чатов: {ex.Message}");
        }
    }

    /// <summary>
    /// Получает чат по его ID.
    /// </summary>
    /// <param name="id">ID чата</param>
    /// <returns>Объект чата или сообщение об ошибке</returns>
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

    /// <summary>
    /// Отправляет сообщение в указанный чат.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="request">Данные сообщения: отправитель, получатель, текст</param>
    /// <returns>Статус отправки</returns>
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

    /// <summary>
    /// Получает все сообщения из заданного чата.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Список сообщений</returns>
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