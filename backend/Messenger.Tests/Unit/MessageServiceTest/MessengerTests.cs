using System;
using System.Threading.Tasks;
using Messenger.Application.Chat.DTOs;
using Messenger.Application.Chat.Services;
using Messenger.Domain.Enums;
using Messenger.Persistence.DbContext;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.Tests.Unit.MessageServiceTest;

public class MessageServiceTests
{
    private MessengerDbContext CreateInMemoryContext()
    {
        var opts = new DbContextOptionsBuilder<MessengerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MessengerDbContext(opts);
    }

    [Fact]
    public async Task SendMessageAsync_Should_SaveMessageToDatabase_And_ReturnDto()
    {
        // Arrange
        var dbContext = CreateInMemoryContext();
        var repository = new MessageRepository(dbContext);
        var service = new MessageService(repository);

        var senderId   = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var chatId     = Guid.NewGuid();

        var request = new SendMessageRequest
        {
            ChatId           = chatId,
            ReceiverId       = receiverId,
            EncryptedContent = "cipher-text",
            EncryptedAesKey  = "encrypted-key",
            Iv               = "initialization-vector",
            Content          = "hello world",
            Type             = MessageType.Text
        };

        // Act
        var dto = await service.SendMessageAsync(senderId, request);

        // Assert: проверим возвращённый DTO
        Assert.Equal(dto.Id, dto.Id);
        Assert.Equal(chatId, dto.ChatId);
        Assert.Equal(senderId, dto.SenderId);
        Assert.Equal(receiverId, dto.ReceiverId);
        Assert.Equal("cipher-text", dto.EncryptedContent);
        Assert.Equal("encrypted-key", dto.EncryptedAesKey);
        Assert.Equal("initialization-vector", dto.Iv);
        Assert.Equal(MessageType.Text, dto.Type);

        // Assert: сообщение действительно сохранено в БД
        var entity = await dbContext.Messages.FindAsync(dto.Id);
        Assert.NotNull(entity);
        Assert.Equal(chatId,         entity.ChatId);
        Assert.Equal(senderId,       entity.SenderId);
        Assert.Equal(receiverId,     entity.ReceiverId);
        Assert.Equal("cipher-text",  entity.EncryptedContent);
        Assert.Equal("encrypted-key",entity.EncryptedAesKey);
        Assert.Equal("initialization-vector", entity.Iv);
        Assert.Equal("hello world",  entity.Content);
        Assert.Equal(MessageType.Text, entity.Type);
        Assert.True(entity.SentAt <= DateTime.UtcNow);
    }
}