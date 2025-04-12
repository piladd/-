using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Messenger.Application.Services;
using Messenger.Domain.Entities;
using Messenger.Tests.TestData;
using Xunit;

namespace Messenger.Tests.Unit;

public class ChatServiceTests
{
    [Fact]
    public async Task CreateChatAsync_ShouldReturnChatWithTitle()
    {
        // Arrange
        var service = new ChatService();
        var title = "Чат для теста";

        // Act
        var chat = await service.CreateChatAsync(title);

        // Assert
        Assert.NotNull(chat);
        Assert.Equal(title, chat.Title);
        Assert.NotEqual(Guid.Empty, chat.Id);
        Assert.NotNull(chat.Messages);
    }

    [Fact]
    public async Task GetAllChatsAsync_ShouldReturnChats()
    {
        // Arrange
        var service = new ChatService();
        await service.CreateChatAsync("Тестовый чат 1");
        await service.CreateChatAsync("Тестовый чат 2");

        // Act
        var result = await service.GetAllChatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetChatByIdAsync_ShouldReturnChat_WhenChatExists()
    {
        // Arrange
        var service = new ChatService();
        var createdChat = await service.CreateChatAsync("Чат по Id");

        // Act
        var result = await service.GetChatByIdAsync(createdChat.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdChat.Id, result?.Id);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldAddMessageToChat()
    {
        // Arrange
        var service = new ChatService();
        var chat = await service.CreateChatAsync("Сообщение в чат");
        var sender = UserTestData.GetSampleUser();
        var receiver = UserTestData.GetSampleUser();
        var messageText = "Привет, это тест";

        // Act
        await service.SendMessageAsync(chat.Id, sender.Id, receiver.Id, messageText);
        var updatedChat = await service.GetChatByIdAsync(chat.Id);

        // Assert
        Assert.NotNull(updatedChat);
        Assert.Single(updatedChat.Messages);
        Assert.Equal(messageText, updatedChat.Messages.First().Content);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessagesForChat()
    {
        // Arrange
        var service = new ChatService();
        var chat = await service.CreateChatAsync("Сообщения");
        var sender = UserTestData.GetSampleUser();
        var receiver = UserTestData.GetSampleUser();

        await service.SendMessageAsync(chat.Id, sender.Id, receiver.Id, "Сообщение 1");
        await service.SendMessageAsync(chat.Id, sender.Id, receiver.Id, "Сообщение 2");

        // Act
        var messages = await service.GetMessagesAsync(chat.Id);

        // Assert
        Assert.Equal(2, messages.Count());
    }
}