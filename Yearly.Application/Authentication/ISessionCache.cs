using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication;

public interface ISessionCache
{
    /// <summary>
    /// Adds the user to the cache.
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <param name="user"></param>
    public void Add(string sessionCookie, User user);
    
    /// <summary>
    /// Returns the cached user. If null, the session is expired and invalid.
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <returns></returns>
    public User? Get(string sessionCookie);
}