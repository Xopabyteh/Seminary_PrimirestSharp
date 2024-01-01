using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication;

public interface ISessionCache
{
    /// <summary>
    /// Adds the user to the cache.
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <param name="userId"></param>
    public Task AddAsync(string sessionCookie, UserId userId);
    
    /// <summary>
    /// Returns the cached user. If null, the session is expired and invalid.
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <returns></returns>
    public Task<UserId?> GetAsync(string sessionCookie);

    /// <summary>
    /// Removes the user from the cache.
    /// </summary>
    public Task RemoveAsync(string sessionCookie);

    //public void InvalidateCache(User newUserData);
}