using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Infrastructure.Repositories
{
    public class ChatRepository
    {
        private readonly MessengerDbContext _dbContext;

        public ChatRepository(MessengerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Chat> CreateAsync(string title)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Title = title,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync();
            return chat;
        }

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await _dbContext.Chats.ToListAsync();
        }

        public async Task<Chat?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Chats.FindAsync(id);
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
        {
            return await _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}