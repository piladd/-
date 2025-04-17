using System.Threading.Tasks;
using Messenger.Domain.Models;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса отправки сообщений.
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Отправляет сообщение от одного пользователя к другому с применением шифрования.
    /// </summary>
    /// <param name="request">Запрос с информацией о сообщении</param>
    /// <returns>Отправленное сообщение</returns>
    Task<Message> SendMessageAsync(SendMessageRequest request);
}