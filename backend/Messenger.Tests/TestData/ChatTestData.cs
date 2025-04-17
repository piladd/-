using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Messenger.Tests.TestData;

/// <summary>
/// Набор тестовых данных для чатов.
/// </summary>
public static class ChatTestData
{
    /// <summary>
    /// Возвращает экземпляр тестового чата.
    /// </summary>
    /// <param name="id">Необязательный ID чата, если не задан — генерируется новый</param>
    /// <returns>Объект чата с пустым списком сообщений</returns>
    public static Chat GetSampleChat(Guid? id = null)
    {
        return new Chat
        {
            Id = id ?? Guid.NewGuid(),
            Title = "Тестовый чат",
            CreatedAt = DateTime.UtcNow,
            Messages = new List<Message>()
        };
    }
}