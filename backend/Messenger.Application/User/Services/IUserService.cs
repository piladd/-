using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messenger.Application.User.DTOs;

namespace Messenger.Application.User.Services;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(Guid userId);
    Task<List<UserSearchResultDto>> SearchUsersAsync(string username);
    Task UpdateDisplayNameAsync(Guid userId, string displayName);
    
    Task SetAvatarUrlAsync(Guid userId, string avatarUrl);
}