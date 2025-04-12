using Microsoft.EntityFrameworkCore;
using Messenger.Infrastructure;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Messenger.Security;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;



namespace Messenger.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly AttachmentRepository _attachmentRepository;
        private readonly EncryptionService _encryptionService;

        public AttachmentService(AttachmentRepository attachmentRepository, EncryptionService encryptionService)
        {
            _attachmentRepository = attachmentRepository;
            _encryptionService = encryptionService;
        }

        public async Task<int> UploadAttachmentAsync(string fileName, byte[] fileData)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Имя файла обязательно.", nameof(fileName));

            try
            {
                var symKey = _encryptionService.GenerateSymmetricKey();
                // Преобразуем байты файла в строку Base64 для шифрования (демонстрационно)
                var fileDataBase64 = Convert.ToBase64String(fileData);
                var (iv, cipherBytes) = _encryptionService.EncryptWithAes(fileDataBase64, symKey);

                var attachment = new Attachment
                {
                    FileName = fileName,
                    EncryptedData = cipherBytes,
                    SymmetricKey = Convert.ToBase64String(symKey),
                    IV = Convert.ToBase64String(iv),
                    CreatedAt = DateTime.UtcNow
                };

                await _attachmentRepository.AddAttachmentAsync(attachment);
                return attachment.Id;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при загрузке вложения.", ex);
            }
        }

        public async Task<byte[]?> DownloadAttachmentAsync(int attachmentId)
        {
            try
            {
                var attachment = await _attachmentRepository.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                    return null;

                var symKey = Convert.FromBase64String(attachment.SymmetricKey);
                var decryptedString = _encryptionService.DecryptWithAes(attachment.EncryptedData, symKey, Convert.FromBase64String(attachment.IV));
                return Convert.FromBase64String(decryptedString);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при скачивании вложения.", ex);
            }
        }
    }
}
