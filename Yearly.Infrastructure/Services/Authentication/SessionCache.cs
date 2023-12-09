using Microsoft.Extensions.Caching.Memory;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Infrastructure.Services.Authentication;

public class SessionCache : ISessionCache
{
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30);

    //In memory cache
    private readonly IMemoryCache _cache;

    public SessionCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Add(string sessionCookie, User user)
    {
        _cache.Set(sessionCookie, user, ExpirationTime);
    }

    public User? Get(string sessionCookie)
    {
        return _cache.Get<User>(sessionCookie);
    }

    public void Remove(string sessionCookie)
    {
        _cache.Remove(sessionCookie);
    }
}