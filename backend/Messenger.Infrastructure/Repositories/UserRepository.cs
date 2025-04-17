using Messenger.Domain.Entities;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Messenger.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для управления пользователями в базе данных.
/// </summary>
public class UserRepository
{
    private readonly MessengerDbContext _dbContext;

    /// <summary>
    /// Конструктор репозитория пользователей.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public UserRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Добавляет нового пользователя в базу данных.
    /// </summary>
    /// <param name="user">Пользователь для добавления</param>
    public async Task AddUserAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Ищет пользователя по имени пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Ищет пользователя по его ID.
    /// </summary>
    /// <param name="id">ID пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }
}