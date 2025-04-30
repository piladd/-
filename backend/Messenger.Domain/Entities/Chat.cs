using System;
using System.Collections.Generic;

namespace Messenger.Domain.Entities;

/// <summary>
/// Диалог 1-на-1
/// </summary>
public class Chat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserAId { get; set; }
    public Guid UserBId { get; set; }
    public string Title { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public List<Guid> ParticipantIds { get; set; }
}