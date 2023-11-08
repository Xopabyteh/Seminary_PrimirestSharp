using Yearly.Domain.Models.UserAgg;

namespace Yearly.Domain.Repositories;

public interface IUserRepository
{
    public Task AddAsync(User user);

    public Task UpdateAsync(User user);

    public Task<User?> GetByIdAsync(int id);

    public Task<bool> DoesUserExistAsync(string username);
}