using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Messenger.Domain.Entities;

namespace Messenger.API.Controllers
{
    [Authorize]
[ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // Отправка сообщения
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Сообщение не может быть пустым.");

            var message = await _messageService.SendMessageAsync(request.ChatId, request.SenderId, request.Content, request.Type);
            return Ok(message);
        }

        // Получение сообщений чата по ID
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetByChatId(Guid chatId)
        {
            var messages = await _messageService.GetMessagesByChatIdAsync(chatId);
            return Ok(messages);
        }
    }

    public class SendMessageRequest
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "Text";
    }
}