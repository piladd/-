using Messenger.Domain.Entities;
using Messenger.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

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

    /// <summary>
    /// Получает вложение по objectName (уникальному имени объекта в хранилище).
    /// </summary>
    public async Task<Attachment?> GetAttachmentByObjectNameAsync(string objectName)
    {
        return await _dbContext.Attachments.FirstOrDefaultAsync(x => x.ObjectName == objectName);
    }
    
    public async Task<(string EncryptedAesKey, string Iv)?> GetMetaAsync(string objectName)
    {
        var result = await _dbContext.Attachments
            .Where(a => a.ObjectName == objectName)
            .Select(a => new { a.EncryptedAesKey, a.Iv })
            .FirstOrDefaultAsync();

        return result == null ? null : (result.EncryptedAesKey, result.Iv);
    }

}