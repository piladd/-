using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Messenger.Infrastructure.Data;

/// <summary>
/// Фабрика для создания контекста базы данных во время разработки и миграций.
/// </summary>
public class MessengerDbContextFactory : IDesignTimeDbContextFactory<MessengerDbContext>
{
    /// <summary>
    /// Создаёт экземпляр MessengerDbContext для использования в миграциях и CLI-инструментах EF Core.
    /// </summary>
    /// <param name="args">Аргументы командной строки</param>
    /// <returns>Экземпляр MessengerDbContext с заданной строкой подключения</returns>
    public MessengerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MessengerDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=messengerdb;Username=postgres;Password=postgres");

        return new MessengerDbContext(optionsBuilder.Options);
    }
}