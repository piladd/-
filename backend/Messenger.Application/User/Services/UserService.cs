using Messenger.Application.User.DTOs;
using Messenger.Domain.Entities;
using Messenger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Application.User.Services;

/// <summary>
/// Сервис работы с профилем пользователя.
/// </summary>
public class UserService : IUserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl
        };
    }

    public async Task<List<UserSearchResultDto>> SearchUsersAsync(string username)
    {
        var users = await _userRepository.Query()
            .Where(u => u.Username.Contains(username))
            .Take(10)
            .ToListAsync();

        return users.Select(u => new UserSearchResultDto
        {
            Id = u.Id,
            Username = u.Username,
            DisplayName = u.DisplayName
        }).ToList();
    }

    public async Task UpdateDisplayNameAsync(Guid userId, string displayName)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        user.DisplayName = displayName;
        await _userRepository.UpdateAsync(user);
    }
    
    public async Task SetAvatarUrlAsync(Guid userId, string avatarUrl)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        user.AvatarUrl = avatarUrl;
        await _userRepository.UpdateAsync(user);
    }

}