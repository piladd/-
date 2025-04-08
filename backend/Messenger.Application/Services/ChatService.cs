using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;

namespace Messenger.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatRepository _chatRepository;

        public ChatService(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Chat> CreateChatAsync(string title)
        {
            return await _chatRepository.CreateAsync(title);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.GetAllAsync();
        }

        public async Task<Chat?> GetChatByIdAsync(Guid id)
        {
            return await _chatRepository.GetByIdAsync(id);
        }
    }
}