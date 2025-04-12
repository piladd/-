using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messenger.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatRepository _chatRepository;
        private readonly UserRepository _userRepository;

        public ChatService(ChatRepository chatRepository, UserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Chat>> GetChatsForUserAsync(Guid userId)
        {
            return await _chatRepository.GetChatsByUserIdAsync(userId);
        }

        public async Task<Chat> CreateChatAsync(Guid userId1, Guid userId2)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                UserIds = new List<Guid> { userId1, userId2 },
                CreatedAt = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(chat);
            return chat;
        }
    }
}