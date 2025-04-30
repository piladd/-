using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messenger.Domain.Entities;
using Messenger.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сообщениями.
/// </summary>
public class MessageRepository
{
    private readonly MessengerDbContext _dbContext;

    public MessageRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Сохраняет новое сообщение.
    /// </summary>
    public async Task AddAsync(Message message)
    {
        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Получает список сообщений между двумя пользователями (1:1).
    /// </summary>
    public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userAId, Guid userBId)
    {
        return await _dbContext.Messages
            .Where(m =>
                (m.SenderId == userAId && m.ReceiverId == userBId) ||
                (m.SenderId == userBId && m.ReceiverId == userAId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}