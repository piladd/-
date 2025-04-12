using System;
using System.Threading.Tasks;
using Messenger.Application.Services;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
            using var context = new MessengerDbContext(options);

namespace Messenger.Tests.Unit
{
    public class MessageServiceTests
    {
        [Fact]
        public async Task SendMessageAsync_ShouldStoreMessage()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var service = new MessageService(context, new Messenger.Security.EncryptionService());

            var chatId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var content = "Hello!";

            // Act
            var result = await service.SendMessageAsync(chatId, senderId, content);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(chatId, result.ChatId);
            Assert.Equal(content, result.Content);
        }
    }
}