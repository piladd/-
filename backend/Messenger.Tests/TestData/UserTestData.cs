
using Messenger.Domain.Entities;
using System;

namespace Messenger.Tests.TestData;

public static class UserTestData
{
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