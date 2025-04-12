using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Messenger.Infrastructure.Repositories
{
    public class AttachmentRepository
    {
        private readonly MessengerDbContext _dbContext;

        public AttachmentRepository(MessengerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAttachmentAsync(Attachment attachment)
        {
            _dbContext.Attachments.Add(attachment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Attachment?> GetAttachmentByIdAsync(int id)
        {
            return await _dbContext.Attachments.FindAsync(id);
        }
    }
}