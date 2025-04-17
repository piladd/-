using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

/// <summary>
/// Контроллер для отправки сообщений через IMessageService.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    /// <summary>
    /// Конструктор контроллера сообщений.
    /// </summary>
    /// <param name="messageService">Сервис отправки сообщений</param>
    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Отправляет сообщение от одного пользователя к другому.
    /// </summary>
    /// <param name="request">Запрос, содержащий отправителя, получателя, текст и ключи</param>
    /// <returns>Информация об отправленном сообщении или ошибка</returns>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] Domain.Models.SendMessageRequest request)
    {
        try
        {
            var result = await _messageService.SendMessageAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при отправке сообщения: {ex.Message}");
        }
    }
}