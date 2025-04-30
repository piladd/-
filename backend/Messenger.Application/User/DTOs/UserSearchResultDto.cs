using System;

namespace Messenger.Application.User.DTOs;

public class UserSearchResultDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}