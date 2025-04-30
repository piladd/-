using System.Threading.Tasks;
using Messenger.Infrastructure;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Messenger.Application.Services;

/// <summary>
/// Сервис для сохранения и получения публичных ключей пользователей.
/// </summary>
public class KeyStoreService : IKeyStoreService
{
    private readonly MessengerDbContext _db;

    /// <summary>
    /// Конструктор сервиса KeyStore, получает контекст базы данных.
    /// </summary>
    /// <param name="db">Контекст базы данных Messenger</param>
    public KeyStoreService(MessengerDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Сохраняет или обновляет публичный ключ пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="keyBase64">Публичный ключ в формате Base64</param>
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

    /// <summary>
    /// Получает публичный ключ пользователя по его ID.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Публичный ключ или null, если не найден</returns>
    public async Task<PublicKey?> GetPublicKeyAsync(Guid userId)
    {
        return await _db.PublicKeys.FirstOrDefaultAsync(k => k.UserId == userId);
    }
}