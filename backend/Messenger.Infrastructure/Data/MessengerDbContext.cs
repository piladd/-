using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Entities;

namespace Messenger.Infrastructure.Data;

public class MessengerDbContext : DbContext
{
    public MessengerDbContext(DbContextOptions<MessengerDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<PublicKey> PublicKeys { get; set; } // 🔧 добавлено!

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.SentMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PublicKey>()
            .HasIndex(p => p.UserId)
            .IsUnique(); // 🔧 добавлено!
    }
}