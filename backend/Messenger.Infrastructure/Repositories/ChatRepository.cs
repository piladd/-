using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories
{
    public class ChatRepository
    {
        private readonly MessengerDbContext _db;

        public ChatRepository(MessengerDbContext db)
        {
            _db = db;
        }

        public async Task<Chat> CreateAsync(string title)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Title = title
            };

            _db.Chats.Add(chat);
            await _db.SaveChangesAsync();
            return chat;
        }

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await _db.Chats
                .Include(c => c.Messages)
                .ToListAsync();
        }

        public async Task<Chat?> GetByIdAsync(Guid id)
        {
            return await _db.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}