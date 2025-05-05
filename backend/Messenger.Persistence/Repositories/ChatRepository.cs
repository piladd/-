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
    public async Task<IEnumerable<Chat>> GetAllAsync(Guid userId)
    {
        return await _dbContext.Chats.ToListAsync();
    }

    /// <summary>
    /// Возвращает чат по его ID.
    /// </summary>
    public async Task<Chat?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Chats.FindAsync(id);
    }

    /// <summary>
    /// Возвращает все сообщения, принадлежащие указанному чату.
    /// </summary>
    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
    {
        return await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    /// <summary>
    /// Добавляет новый чат в базу данных.
    /// </summary>
    public async Task AddAsync(Chat chat)
    {
        _dbContext.Chats.Add(chat);
        await _dbContext.SaveChangesAsync();
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

    /// <summary>
    /// Находит приватный чат между двумя пользователями.
    /// </summary>
    public async Task<Chat?> GetPrivateChatAsync(Guid userA, Guid userB)
    {
        return await _dbContext.Chats
            .FirstOrDefaultAsync(c =>
                c.ParticipantIds.Contains(userA) &&
                c.ParticipantIds.Contains(userB) &&
                c.ParticipantIds.Count == 2);
    }
}
