using Messenger.Domain.Entities;
using Messenger.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

/// <summary>
/// Репозиторий для управления чатами и сообщениями в базе данных.
/// </summary>
public class ChatRepository
{
    private readonly MessengerDbContext _dbContext;

    /// <summary>
    /// Конструктор репозитория чатов.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public ChatRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Создаёт новый чат с указанным названием.
    /// </summary>
    /// <param name="title">Название чата</param>
    /// <returns>Созданный объект чата</returns>
    public async Task<Chat> CreateAsync(string title)
    {
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Title = title,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Chats.Add(chat);
        await _dbContext.SaveChangesAsync();
        return chat;
    }

    /// <summary>
    /// Возвращает все чаты из базы данных.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Коллекция чатов</returns>
    public async Task<IEnumerable<Chat>> GetAllAsync(Guid userId)
    {
        return await _dbContext.Chats.ToListAsync();
    }

    /// <summary>
    /// Возвращает чат по его ID.
    /// </summary>
    /// <param name="id">ID чата</param>
    /// <returns>Чат или null, если не найден</returns>
    public async Task<Chat?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Chats.FindAsync(id);
    }

    /// <summary>
    /// Возвращает все сообщения, принадлежащие указанному чату.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Коллекция сообщений, отсортированная по времени отправки</returns>
    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
    {
        return await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    /// <summary>
    /// Не используется, заглушка.
    /// </summary>
    public Task AddAsync(Chat chat)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Сохраняет сообщение в базу.
    /// </summary>
    public async Task SaveMessageAsync(Guid chatId, Message message)
    {
        message.ChatId = chatId;
        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Получает все сообщения между двумя пользователями.
    /// </summary>
    public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid senderId, Guid receiverId)
    {
        return await _dbContext.Messages
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId)
                     || (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}
