using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Entities;
using Attachment = Messenger.Domain.Entities.Attachment;
using PublicKey = Messenger.Domain.Entities.PublicKey;

namespace Messenger.Persistence.DbContext;

/// <summary>
/// Контекст базы данных мессенджера.
/// </summary>
public class MessengerDbContext(DbContextOptions<MessengerDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<PublicKey> PublicKeys { get; set; }
    public DbSet<Chat> Chats { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Один пользователь может отправить много сообщений
        modelBuilder.Entity<User>()
            .HasMany(u => u.SentMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Уникальный публичный ключ на пользователя
        modelBuilder.Entity<PublicKey>()
            .HasIndex(p => p.UserId)
            .IsUnique();
    }
}