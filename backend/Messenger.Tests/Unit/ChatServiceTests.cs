using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Messenger.Application.Services;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Messenger.Tests.Unit;

public class ChatServiceTests
{
    [Fact]
    public async Task CreateChatAsync_ShouldReturnNewChat()
    {
        // Arrange
        var mockChatRepo = new Mock<ChatRepository>(null as object);
        var mockUserRepo = new Mock<UserRepository>(null as object);
        var service = new ChatService(mockChatRepo.Object, mockUserRepo.Object);

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        mockChatRepo.Setup(r => r.AddAsync(It.IsAny<Chat>())).Returns(Task.CompletedTask);

        // Act
        var result = await service.CreateChatAsync(userId1, userId2);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(userId1, result.UserIds);
        Assert.Contains(userId2, result.UserIds);
    }
}