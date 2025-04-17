using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Messenger.Application.Services;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using Messenger.Tests.TestData;
using Xunit;

namespace Messenger.Tests.Unit;

public class ChatServiceTests
{
    private readonly InMemoryChatRepository _repo;
    private readonly ChatService _service;
    private readonly Guid _testUserId;
    private readonly Guid _otherUserId;

    public ChatServiceTests()
    {
        _repo = new InMemoryChatRepository();
        _service = new ChatService(_repo);
        _repo.Clear();

        _testUserId = Guid.NewGuid();
        _otherUserId = Guid.NewGuid();
    }

    [Fact]
    public async Task CreateChatAsync_ShouldReturnChatWithTitleAndParticipants()
    {
        // Arrange
        var title = "Чат для теста";
        var participants = new List<Guid> { _testUserId, _otherUserId };

        // Act
        var chat = await _service.CreateChatAsync(title, participants);

        // Assert
        Assert.NotNull(chat);
        Assert.Equal(title, chat.Title);
        Assert.NotEqual(Guid.Empty, chat.Id);
        Assert.Equal(2, chat.ParticipantIds.Count);
        Assert.Contains(_testUserId, chat.ParticipantIds);
        Assert.Contains(_otherUserId, chat.ParticipantIds);
        Assert.Empty(chat.Messages);
    }

    [Fact]
    public async Task GetAllChatsAsync_ShouldReturnOnlyChatsOfUser()
    {
        // Arrange
        await _service.CreateChatAsync("Чат A", new List<Guid> { _testUserId });
        await _service.CreateChatAsync("Чат B", new List<Guid> { _testUserId, _otherUserId });
        await _service.CreateChatAsync("Чат C", new List<Guid> { _otherUserId }); // чужой чат

        // Act
        var userChats = (await _service.GetAllChatsAsync(_testUserId)).ToList();
        var otherUserChats = (await _service.GetAllChatsAsync(_otherUserId)).ToList();

        // Assert
        Assert.Equal(2, userChats.Count);
        Assert.Equal(2, otherUserChats.Count); // ✅ ожидание двух чатов

        Assert.All(userChats, chat => Assert.Contains(_testUserId, chat.ParticipantIds));
        Assert.All(otherUserChats, chat => Assert.Contains(_otherUserId, chat.ParticipantIds));
    }

    [Fact]
    public async Task GetChatByIdAsync_ShouldReturnCorrectChat()
    {
        // Arrange
        var chat = await _service.CreateChatAsync("Чат по Id", new List<Guid> { _testUserId });

        // Act
        var found = await _service.GetChatByIdAsync(chat.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(chat.Id, found?.Id);
        Assert.Equal(chat.Title, found?.Title);
        Assert.Contains(_testUserId, found!.ParticipantIds);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldAddMessageToChat()
    {
        // Arrange
        var chat = await _service.CreateChatAsync("Сообщение в чат", new List<Guid> { _testUserId, _otherUserId });
        var sender = UserTestData.GetSampleUser(_testUserId);
        var receiver = UserTestData.GetSampleUser(_otherUserId);
        var messageText = "Привет, это тест";

        // Act
        await _service.SendMessageAsync(chat.Id, sender.Id, receiver.Id, messageText);
        var updatedChat = await _service.GetChatByIdAsync(chat.Id);

        // Assert
        Assert.NotNull(updatedChat);
        var messages = updatedChat!.Messages;
        Assert.Single(messages);

        var msg = messages.First();
        Assert.Equal(messageText, msg.Content);
        Assert.Equal(sender.Id, msg.SenderId);
        Assert.Equal(chat.Id, msg.ChatId);
        Assert.True(msg.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldNotFail_WhenChatNotFound()
    {
        // Arrange
        var fakeChatId = Guid.NewGuid();
        var sender = UserTestData.GetSampleUser();
        var receiver = UserTestData.GetSampleUser();

        // Act & Assert — должен просто ничего не сделать, не выбрасывать исключение
        await _service.SendMessageAsync(fakeChatId, sender.Id, receiver.Id, "Сообщение");
        var result = await _service.GetChatByIdAsync(fakeChatId);

        Assert.Null(result); // подтверждение, что чат не появился
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessagesInCorrectOrder()
    {
        // Arrange
        var chat = await _service.CreateChatAsync("Сообщения", new List<Guid> { _testUserId });
        var sender = UserTestData.GetSampleUser(_testUserId);
        var receiver = UserTestData.GetSampleUser();

        await _service.SendMessageAsync(chat.Id, sender.Id, receiver.Id, "Первое сообщение");
        await Task.Delay(10); // на случай одинакового timestamp
        await _service.SendMessageAsync(chat.Id, sender.Id, receiver.Id, "Второе сообщение");

        // Act
        var messages = (await _service.GetMessagesAsync(chat.Id)).ToList();

        // Assert
        Assert.Equal(2, messages.Count);
        Assert.Equal("Первое сообщение", messages[0].Content);
        Assert.Equal("Второе сообщение", messages[1].Content);
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnEmpty_WhenChatDoesNotExist()
    {
        // Arrange
        var fakeChatId = Guid.NewGuid();

        // Act
        var messages = await _service.GetMessagesAsync(fakeChatId);

        // Assert
        Assert.Empty(messages);
    }
}