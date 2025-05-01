namespace Messenger.Application.Interfaces;

public interface IUserRepository
{
    Task AddUserAsync(Domain.Entities.User user);
    Task<Domain.Entities.User?> GetByUsernameAsync(string username);
    Task<Domain.Entities.User?> GetByIdAsync(Guid id);
    Task UpdateAsync(Domain.Entities.User user);
    IQueryable<Domain.Entities.User> Query();
}