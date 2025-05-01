using System;

namespace Messenger.Application.User.DTOs;

public class UserSearchResultDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? DisplayName { get; set; } = null!;
}