using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Messenger.Persistence.DbContext;

namespace Messenger.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с вложениями (файлами) в базе данных.
/// </summary>
public class AttachmentRepository
{
    private readonly MessengerDbContext _dbContext;

    /// <summary>
    /// Конструктор репозитория вложений.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public AttachmentRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Добавляет новое вложение в базу данных.
    /// </summary>
    /// <param name="attachment">Вложение для добавления</param>
    public async Task AddAttachmentAsync(Attachment attachment)
    {
        _dbContext.Attachments.Add(attachment);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Получает вложение по его ID.
    /// </summary>
    /// <param name="id">ID вложения</param>
    /// <returns>Объект вложения или null, если не найден</returns>
    public async Task<Attachment?> GetAttachmentByIdAsync(int id)
    {
        return await _dbContext.Attachments.FindAsync(id);
    }
}