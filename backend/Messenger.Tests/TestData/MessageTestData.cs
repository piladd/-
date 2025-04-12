
using Messenger.Domain.Entities;
using System;

namespace Messenger.Tests.TestData;

public static class MessageTestData
{
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