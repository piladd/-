using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Messenger.Infrastructure.Data
{
    public class MessengerDbContextFactory : IDesignTimeDbContextFactory<MessengerDbContext>
    {
        public MessengerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessengerDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=messengerdb;Username=postgres;Password=postgres");

            return new MessengerDbContext(optionsBuilder.Options);
        }
    }
}