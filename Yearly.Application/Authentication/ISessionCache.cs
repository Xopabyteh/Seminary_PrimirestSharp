using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication;

public interface ISessionCache
{
    /// <summary>
    /// Adds the user to the cache.
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <param name="user"></param>
    public Task AddAsync(string sessionCookie, User user);
    
    ///// <summary>
    ///// Returns the cached user. If null, the session is expired and invalid.
    ///// </summary>
    ///// <param name="sessionCookie"></param>
    ///// <returns></returns>
    //public Task<UserId?> GetAsync(string sessionCookie);

    public Task<User?> GetAsync(string sessionCookie);

    /// <summary>
    /// Removes the user from the cache.
    /// </summary>
    public Task RemoveAsync(string sessionCookie);

    /// <summary>
    /// The time at which the session expires. Not the *delay*, but the **date**
    /// </summary>
    public DateTimeOffset SessionExpiration { get; }
}