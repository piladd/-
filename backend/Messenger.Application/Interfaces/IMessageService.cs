using System.Threading.Tasks;
using Messenger.Domain.Models;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface IMessageService
{
    Task<Message> SendMessageAsync(SendMessageRequest request);
}