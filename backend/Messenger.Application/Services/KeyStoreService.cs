using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Application.Services;

public class KeyStoreService : IKeyStoreService
{
    private readonly MessengerDbContext _db;

    public KeyStoreService(MessengerDbContext db)
    {
        _db = db;
    }

    public async Task SavePublicKeyAsync(Guid userId, string keyBase64)
    {
        var existing = await _db.PublicKeys.FirstOrDefaultAsync(k => k.UserId == userId);

        if (existing != null)
        {
            existing.KeyBase64 = keyBase64;
            existing.UploadedAt = DateTime.UtcNow;
        }
        else
        {
            _db.PublicKeys.Add(new PublicKey
            {
                UserId = userId,
                KeyBase64 = keyBase64
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<PublicKey?> GetPublicKeyAsync(Guid userId)
    {
        return await _db.PublicKeys.FirstOrDefaultAsync(k => k.UserId == userId);
    }
}