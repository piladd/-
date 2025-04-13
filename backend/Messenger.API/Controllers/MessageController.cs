using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Messenger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

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