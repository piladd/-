using Messenger.Domain.Entities;
using Messenger.Persistence.Repositories;

namespace Messenger.Application.Chat.Services;

/// <summary>
/// Сервис для управления чатами и отправки сообщений.
/// </summary>
public class ChatService : IChatService
{
    private readonly ChatRepository _repo;

    /// <summary>
    /// Конструктор сервиса чатов.
    /// </summary>
    /// <param name="repo">Репозиторий чатов</param>
    public ChatService(ChatRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Получает все чаты, в которых участвует указанный пользователь.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список чатов</returns>
    public async Task<IEnumerable<Domain.Entities.Chat>> GetAllChatsAsync(Guid userId) =>
        await _repo.GetAllAsync(userId);

    /// <summary>
    /// Получает чат по его ID.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Объект чата или null, если не найден</returns>
    public async Task<Domain.Entities.Chat?> GetChatByIdAsync(Guid chatId) =>
        await _repo.GetByIdAsync(chatId);

    /// <summary>
    /// Создаёт новый чат с заданным названием и списком участников.
    /// </summary>
    /// <param name="name">Название чата</param>
    /// <param name="participantIds">Список ID участников</param>
    /// <returns>Созданный чат</returns>
    public async Task<Domain.Entities.Chat> CreateChatAsync(string name, List<Guid> participantIds)
    {
        var chat = new Domain.Entities.Chat
        {
            Id = Guid.NewGuid(),
            Title = name,
            CreatedAt = DateTime.UtcNow,
            Messages = new List<Message>(),
            ParticipantIds = participantIds
        };

        await _repo.AddAsync(chat);
        return chat;
    }

    /// <summary>
    /// Отправляет сообщение в указанный чат.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="senderId">ID отправителя</param>
    /// <param name="messageId">ID сообщения</param>
    public async Task SendMessageAsync(Guid chatId, Guid senderId, Guid messageId)
    {
        var message = new Message
        {
            Id = messageId,
            ChatId = chatId,
            SenderId = senderId,
            SentAt = DateTime.UtcNow
        };

        await _repo.SaveMessageAsync(chatId, message);
    }

    /// <summary>
    /// Получает все сообщения из указанного чата.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Список сообщений</returns>
    public async Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId)
    {
        var chat = await _repo.GetByIdAsync(chatId);
        return chat?.Messages ?? Enumerable.Empty<Message>();
    }
}
