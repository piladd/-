using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface IKeyStoreService
{
    Task SavePublicKeyAsync(Guid userId, string keyBase64);
    Task<PublicKey?> GetPublicKeyAsync(Guid userId);
}