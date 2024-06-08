using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication;

public interface ISessionCache
{
    /// <summary>
    /// Associates user with session in cache
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <param name="user"></param>
    /// <returns>UTC Time when cookie expires</returns>
    public Task<DateTimeOffset> SetAsync(string sessionCookie, User user);
    
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

    ///// <summary>
    ///// The time at which the session expires. Not the *delay*, but the **date**
    ///// </summary>
    //public DateTimeOffset SessionExpiration { get; }
}