using System.Threading.Tasks;

namespace Messenger.Application.Interfaces;

public interface IAttachmentService
{
    Task<int> UploadAttachmentAsync(string fileName, byte[] fileData);
    Task<byte[]?> DownloadAttachmentAsync(int attachmentId);
}