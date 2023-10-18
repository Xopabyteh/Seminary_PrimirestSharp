using ErrorOr;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<User>> GetByIdAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DoesUserExistAsync(string username)
    {
        throw new NotImplementedException();
    }
}