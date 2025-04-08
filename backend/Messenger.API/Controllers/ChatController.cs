using Messenger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Название чата обязательно.");

            var chat = await _chatService.CreateChatAsync(request.Title);
            return Ok(chat);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var chats = await _chatService.GetAllChatsAsync();
            return Ok(chats);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            return chat is not null ? Ok(chat) : NotFound("Чат не найден.");
        }
    }

    public class ChatRequest
    {
        public string Title { get; set; } = string.Empty;
    }
}