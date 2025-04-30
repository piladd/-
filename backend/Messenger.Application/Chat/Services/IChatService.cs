using Messenger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с чатами и сообщениями.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Возвращает все чаты, в которых участвует указанный пользователь.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список чатов</returns>
    Task<IEnumerable<Domain.Entities.Chat>> GetAllChatsAsync(Guid userId);

    /// <summary>
    /// Возвращает чат по его ID.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Объект чата или null, если не найден</returns>
    Task<Domain.Entities.Chat?> GetChatByIdAsync(Guid chatId);

    /// <summary>
    /// Создаёт новый чат с заданным названием и участниками.
    /// </summary>
    /// <param name="name">Название чата</param>
    /// <param name="participantIds">Список ID участников</param>
    /// <returns>Созданный чат</returns>
    Task<Domain.Entities.Chat> CreateChatAsync(string name, List<Guid> participantIds);

    /// <summary>
    /// Отправляет сообщение в указанный чат.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <param name="senderId">ID отправителя</param>
    /// <param name="messageId">ID сообщения</param>
    /// <param name="content">Текст сообщения</param>
    Task SendMessageAsync(Guid chatId, Guid senderId, Guid messageId, string content);

    /// <summary>
    /// Возвращает все сообщения в указанном чате.
    /// </summary>
    /// <param name="chatId">ID чата</param>
    /// <returns>Список сообщений</returns>
    Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId);
}