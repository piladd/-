using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Messenger.Infrastructure.Data;
using Messenger.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly MessengerDbContext _db;
        private readonly EncryptionService _encryptionService;

        public MessageService(MessengerDbContext db, EncryptionService encryptionService)
        {
            _db = db;
            _encryptionService = encryptionService;
        }

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

        public async Task<List<Message>> GetMessagesAsync(Guid chatId)
        {
            return await _db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}