using System;
using System.Collections.Generic;

namespace Messenger.Domain.Entities;

/// <summary>
/// Пользователь системы
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PublicKey { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }

    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
}