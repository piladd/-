using Messenger.Domain.Entities;
using System;

namespace Messenger.Tests.TestData;

/// <summary>
/// Набор тестовых данных для сообщений.
/// </summary>
public static class MessageTestData
{
    /// <summary>
    /// Возвращает пример сообщения с заданным или случайным ID.
    /// </summary>
    /// <param name="id">Необязательный ID сообщения</param>
    /// <returns>Объект тестового сообщения</returns>
    public static Message GetSampleMessage(Guid? id = null)
    {
        return new Message
        {
            Id = id ?? Guid.NewGuid(),
            Content = "Пример сообщения",
            Timestamp = DateTime.UtcNow,
            SentAt = DateTime.UtcNow
        };
    }
}