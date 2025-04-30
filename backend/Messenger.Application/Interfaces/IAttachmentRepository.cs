using System.Threading.Tasks;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface IAttachmentRepository
{
    Task AddAttachmentAsync(Domain.Entities.Attachment attachment);
    Task<Domain.Entities.Attachment?> GetAttachmentByIdAsync(int id);
}