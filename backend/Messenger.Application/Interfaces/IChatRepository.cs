using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Интерфейс репозитория для работы с чатами и сообщениями.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// Получает все чаты, в которых участвует указанный пользователь.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Коллекция чатов</returns>
    Task<IEnumerable<Chat>> GetAllAsync(Guid userId);

    /// <summary>
    /// Возвращает чат по его уникальному идентификатору.
    /// </summary>
    /// <param name="id">ID чата</param>
    /// <returns>Объект чата или null, если не найден</returns>
    Task<Chat?> GetByIdAsync(Guid id);

    /// <summary>
    /// Добавляет новый чат в хранилище.
    /// </summary>
    /// <param name="chat">Чат, который нужно сохранить</param>
    Task AddAsync(Chat chat);

    /// <summary>
    /// Добавляет сообщение в чат.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="message">Сообщение для добавления</param>
    Task SaveMessageAsync(Guid chatId, Message message);

    /// <summary>
    /// Очищает все сохранённые чаты. Используется для тестов.
    /// </summary>
    void Clear();
}