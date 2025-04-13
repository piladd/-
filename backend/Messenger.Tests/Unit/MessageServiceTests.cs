using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messenger.Application.Services;
using Messenger.Domain.Models;
using Messenger.Tests.TestData;
using Xunit;

namespace Messenger.Tests.Unit;

public class MessageServiceTests
{
    [Fact]
    public async Task SendMessageAsync_ShouldReturnValidMessage()
    {
        // Arrange
        var service = new MessageService();

        var sender = UserTestData.GetSampleUser();
        var receiver = UserTestData.GetSampleUser();
        var request = new SendMessageRequest
        {
            SenderId = sender.Id.ToString(),
            ReceiverId = receiver.Id.ToString(),
            Content = "Тестовое сообщение"
        };

        // Act
        var result = await service.SendMessageAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Content, result.Content);
        Assert.Equal(Guid.Parse(request.SenderId), result.SenderId);
        Assert.Equal(Guid.Parse(request.ReceiverId), result.ReceiverId);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.True((DateTime.UtcNow - result.Timestamp).TotalSeconds < 5);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnAllMessages()
    {
        // Arrange
        var service = new MessageService();

        var sender = UserTestData.GetSampleUser();
        var receiver = UserTestData.GetSampleUser();

        await service.SendMessageAsync(new SendMessageRequest
        {
            SenderId = sender.Id.ToString(),
            ReceiverId = receiver.Id.ToString(),
            Content = "Первое сообщение"
        });

        await service.SendMessageAsync(new SendMessageRequest
        {
            SenderId = sender.Id.ToString(),
            ReceiverId = receiver.Id.ToString(),
            Content = "Второе сообщение"
        });

        // Act
        var messages = service.GetMessages();

        // Assert
        Assert.NotNull(messages);
        Assert.Equal(2, messages.Count());
        Assert.Contains(messages, m => m.Content == "Первое сообщение");
        Assert.Contains(messages, m => m.Content == "Второе сообщение");
    }
}