using Messenger.Domain.Entities;
using Messenger.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сообщениями.
/// </summary>
public class MessageRepository(MessengerDbContext dbContext)
{
    /// <summary>
    /// Сохраняет новое сообщение.
    /// </summary>
    public async Task AddAsync(Message message)
    {
        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Получает список сообщений между двумя пользователями (1:1).
    /// </summary>
    public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userAId, Guid userBId)
    {
        return await dbContext.Messages
            .Where(m =>
                (m.SenderId == userAId && m.ReceiverId == userBId) ||
                (m.SenderId == userBId && m.ReceiverId == userAId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
    
    public async Task UpdateStatusAsync(Guid messageId, MessageStatus status)
    {
        var message = await dbContext.Messages.FindAsync(messageId);
        if (message is null) return;

        message.Status = status;
        await dbContext.SaveChangesAsync();
    }

}