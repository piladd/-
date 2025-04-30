using System;
using System.Threading.Tasks;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для хранения и получения публичных ключей пользователей.
/// </summary>
public interface IKeyStoreService
{
    /// <summary>
    /// Сохраняет публичный ключ пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="keyBase64">Публичный ключ в виде строки Base64</param>
    Task SavePublicKeyAsync(Guid userId, string keyBase64);

    /// <summary>
    /// Получает публичный ключ пользователя по его ID.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Объект публичного ключа или null, если не найден</returns>
    Task<PublicKey?> GetPublicKeyAsync(Guid userId);
}