using Messenger.Domain.Entities;
using System;

namespace Messenger.Tests.TestData;

/// <summary>
/// Набор тестовых данных для пользователей.
/// </summary>
public static class UserTestData
{
    /// <summary>
    /// Возвращает тестового пользователя с указанным или случайным ID.
    /// </summary>
    /// <param name="id">Необязательный Guid ID</param>
    /// <returns>Объект пользователя</returns>
    public static User GetSampleUser(Guid? id = null)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Username = "testuser",
            Password = "testpass",
            PublicKey = "base64publickey",
            PrivateKey = "base64privatekey"
        };
    }
}