using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Messenger.Tests.TestData;

public static class ChatTestData
{
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