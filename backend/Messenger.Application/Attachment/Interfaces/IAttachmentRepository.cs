using System.Threading.Tasks;
using Messenger.Domain.Entities;

namespace Messenger.Application.Attachment.Interfaces;

/// <summary>
/// Интерфейс репозитория для работы с вложениями.
/// </summary>
public interface IAttachmentRepository
{
    /// <summary>
    /// Добавляет вложение в базу.
    /// </summary>
    Task AddAttachmentAsync(Domain.Entities.Attachment attachment);

    /// <summary>
    /// Получает вложение по ID.
    /// </summary>
    Task<Domain.Entities.Attachment?> GetAttachmentByIdAsync(int id);
}