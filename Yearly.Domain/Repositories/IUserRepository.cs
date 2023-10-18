using ErrorOr;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Repositories;

public interface IUserRepository
{
    public Task AddAsync(User user);

    /// <summary>
    /// It is a domain rule that the users are created upon login, so they should always exist.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ErrorOr<User>> GetByIdAsync(UserId id);

    public Task<bool> DoesUserExistAsync(string username);
}