using System;
using System.Linq;
using System.Threading.Tasks;
using Messenger.Domain.Entities;
using Messenger.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с пользователями.
/// </summary>
public class UserRepository
{
    private readonly MessengerDbContext _dbContext;

    public UserRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Добавляет нового пользователя в базу данных.
    /// </summary>
    public async Task AddUserAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Возвращает пользователя по имени пользователя.
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Возвращает пользователя по идентификатору.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Обновляет пользователя в базе данных.
    /// </summary>
    public async Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Возвращает IQueryable для построения запросов (например, поиск).
    /// </summary>
    public IQueryable<User> Query()
    {
        return _dbContext.Users.AsQueryable();
    }
}