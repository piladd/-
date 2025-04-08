using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Enums;

namespace Messenger.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly MessengerDbContext _db;

        public MessageService(MessengerDbContext db)
        {
            _db = db;
        }

        // Реализация метода отправки сообщения
        public async Task<Message> SendMessageAsync(Guid chatId, Guid senderId, string content, string type = "Text")
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                Type = Enum.TryParse<MessageType>(type, out var messageType) ? messageType : MessageType.Text,
                SentAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        // Получение сообщений по ID чата
        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
        {
            return await _db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}