using Microsoft.EntityFrameworkCore;
using Messenger.Domain.Entities;

namespace Messenger.Infrastructure.Data;

/// <summary>
/// Контекст базы данных мессенджера.
/// Описывает таблицы и связи между сущностями.
/// </summary>
public class MessengerDbContext : DbContext
{
    /// <summary>
    /// Конструктор, принимающий настройки контекста.
    /// </summary>
    public MessengerDbContext(DbContextOptions<MessengerDbContext> options) : base(options) { }

    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Таблица чатов.
    /// </summary>
    public DbSet<Chat> Chats { get; set; }

    /// <summary>
    /// Таблица сообщений.
    /// </summary>
    public DbSet<Message> Messages { get; set; }

    /// <summary>
    /// Таблица вложений (файлов).
    /// </summary>
    public DbSet<Attachment> Attachments { get; set; }

    /// <summary>
    /// Таблица публичных ключей.
    /// </summary>
    public DbSet<PublicKey> PublicKeys { get; set; }

    /// <summary>
    /// Конфигурация связей и ограничений между таблицами.
    /// </summary>
    /// <param name="modelBuilder">Объект построения модели</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Один пользователь может отправить много сообщений
        modelBuilder.Entity<User>()
            .HasMany(u => u.SentMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Один чат содержит много сообщений
        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Один публичный ключ привязан к одному пользователю
        modelBuilder.Entity<PublicKey>()
            .HasIndex(p => p.UserId)
            .IsUnique();
    }
}
