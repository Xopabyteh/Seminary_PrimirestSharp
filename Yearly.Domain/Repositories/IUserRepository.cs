using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Repositories;

public interface IUserRepository
{
    public Task AddAsync(User user);

    public Task UpdateAsync(User user);

    public Task<User?> GetByIdAsync(UserId id);

    public Task<bool> DoesUserExistAsync(string username);
}