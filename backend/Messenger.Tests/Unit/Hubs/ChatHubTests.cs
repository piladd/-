using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;
using Messenger.API.Hubs;
using Messenger.Application.Chat.Services;
using Messenger.Application.Chat.DTOs;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Tests.Unit.Hubs
{
    public class ChatHubTests
    {
        [Fact]
        public async Task SendMessage_Should_InvokeService_And_SendToCallerAndReceiver()
        {
            // Arrange
            var senderId    = Guid.NewGuid();
            var receiverId  = Guid.NewGuid();
            var chatId      = Guid.NewGuid();
            var senderConn  = "conn-sender";
            var receiverConn= "conn-receiver";

            // DTO, который вернёт IMessageService
            var returnedDto = new MessageDto
            {
                Id               = Guid.NewGuid(),
                ChatId           = chatId,
                SenderId         = senderId,
                ReceiverId       = receiverId,
                EncryptedContent = "enc",
                EncryptedAesKey  = "key",
                Iv               = "iv",
                SentAt           = DateTime.UtcNow,
                Status           = MessageStatus.Sent,
                Type             = MessageType.Text
            };

            // Мок IMessageService
            var msgServiceMock = new Mock<IMessageService>();
            msgServiceMock
                .Setup(s => s.SendMessageAsync(
                    senderId,
                    It.Is<SendMessageRequest>(r =>
                        r.ChatId           == chatId &&
                        r.ReceiverId       == receiverId &&
                        r.EncryptedContent == "enc" &&
                        r.EncryptedAesKey  == "key" &&
                        r.Iv               == "iv" &&
                        r.Content          == "plain" &&
                        r.Type             == returnedDto.Type
                    )))
                .ReturnsAsync(returnedDto);

            // Моки IHubCallerClients и прокси
            var clientsMock   = new Mock<IHubCallerClients>();
            var callerProxy   = new Mock<ISingleClientProxy>();
            var receiverProxy = new Mock<ISingleClientProxy>();

            // Caller и Client(senderConn) → callerProxy
            clientsMock.Setup(c => c.Caller).Returns(callerProxy.Object);
            clientsMock.Setup(c => c.Client(senderConn)).Returns(callerProxy.Object);
            // Любой другой Client(...) → receiverProxy
            clientsMock
                .Setup(c => c.Client(It.Is<string>(id => id != senderConn)))
                .Returns(receiverProxy.Object);

            // Создаём хаб
            var hub = new ChatHub(msgServiceMock.Object)
            {
                Clients = clientsMock.Object
            };

            // --- Эмуляция подключения первого пользователя (sender) ---
            var ctxSender = new Mock<HubCallerContext>();
            ctxSender.SetupGet(c => c.UserIdentifier).Returns(senderId.ToString());
            ctxSender.SetupGet(c => c.ConnectionId).Returns(senderConn);
            ctxSender.SetupGet(c => c.User).Returns(new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, senderId.ToString()) },
                    "test")));
            hub.Context = ctxSender.Object;
            await hub.OnConnectedAsync();

            // --- Эмуляция подключения второго пользователя (receiver) ---
            var ctxReceiver = new Mock<HubCallerContext>();
            ctxReceiver.SetupGet(c => c.UserIdentifier).Returns(receiverId.ToString());
            ctxReceiver.SetupGet(c => c.ConnectionId).Returns(receiverConn);
            ctxReceiver.SetupGet(c => c.User).Returns(new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, receiverId.ToString()) },
                    "test")));
            hub.Context = ctxReceiver.Object;
            await hub.OnConnectedAsync();

            // Вернём контекст к отправителю
            hub.Context = ctxSender.Object;

            // Act
            await hub.SendMessage(
                chatId,
                receiverId,
                returnedDto.EncryptedContent,
                returnedDto.EncryptedAesKey,
                returnedDto.Iv,
                "plain",
                (int)returnedDto.Type
            );

            // Assert
            msgServiceMock.Verify(s =>
                s.SendMessageAsync(senderId, It.IsAny<SendMessageRequest>()),
                Times.Once);

            // Отправитель (Caller) получил ReceiveMessage
            callerProxy.Verify(c =>
                c.SendCoreAsync(
                    "ReceiveMessage",
                    It.Is<object[]>(o => o.Length == 1 && o[0] == returnedDto),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Получатель (Client(receiverConn)) получил ReceiveMessage
            receiverProxy.Verify(c =>
                c.SendCoreAsync(
                    "ReceiveMessage",
                    It.Is<object[]>(o => o.Length == 1 && o[0] == returnedDto),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
