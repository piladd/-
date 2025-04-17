using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Tests.TestData;

/// <summary>
/// In-memory реализация репозитория чатов, используется для юнит-тестов без базы данных.
/// </summary>
public class InMemoryChatRepository : IChatRepository
{
    /// <summary>
    /// Внутренний список чатов, хранящийся в памяти.
    /// </summary>
    private readonly List<Chat> _chats = new();

    /// <summary>
    /// Возвращает все чаты, в которых участвует указанный пользователь.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список чатов</returns>
    public Task<IEnumerable<Chat>> GetAllAsync(Guid userId)
    {
        var result = _chats.Where(c => c.ParticipantIds.Contains(userId));
        return Task.FromResult(result.AsEnumerable());
    }

    /// <summary>
    /// Возвращает чат по его ID.
    /// </summary>
    /// <param name="id">ID чата</param>
    /// <returns>Объект чата или null</returns>
    public Task<Chat?> GetByIdAsync(Guid id) =>
        Task.FromResult(_chats.FirstOrDefault(c => c.Id == id));

    /// <summary>
    /// Добавляет чат в список. Требует наличие хотя бы одного участника.
    /// </summary>
    /// <param name="chat">Добавляемый чат</param>
    public Task AddAsync(Chat chat)
    {
        if (chat.ParticipantIds.Count == 0)
            throw new InvalidOperationException("В чате должен быть хотя бы один участник.");

        _chats.Add(chat);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Добавляет сообщение в указанный чат.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="message">Сообщение</param>
    public Task SaveMessageAsync(Guid chatId, Message message)
    {
        var chat = _chats.FirstOrDefault(c => c.Id == chatId);
        chat?.Messages?.Add(message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Очищает все чаты в памяти (для сброса состояния между тестами).
    /// </summary>
    public void Clear() => _chats.Clear();
}
