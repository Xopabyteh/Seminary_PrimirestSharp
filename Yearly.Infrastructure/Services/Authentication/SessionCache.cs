using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Yearly.Application.Authentication;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Authentication;

public class SessionCache : ISessionCache
{ 
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30);

    private readonly IDistributedCache _userCache;

    public SessionCache(IDistributedCache userCache)
    {
        _userCache = userCache;
    }

    public async Task AddAsync(string sessionCookie, UserId userId)
    {
        var userDataJson = JsonConvert.SerializeObject(userId);

        var options = new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = ExpirationTime};
        await _userCache.SetStringAsync(sessionCookie, userDataJson, options);
    }

    public async Task<UserId?> GetAsync(string sessionCookie)
    {
        var userDataJson = await _userCache.GetStringAsync(sessionCookie);
        if (string.IsNullOrEmpty(userDataJson))
            return null;

        var userId = JsonConvert.DeserializeObject<UserId>(userDataJson);
        return userId;
    }

    public Task RemoveAsync(string sessionCookie)
    {
        return _userCache.RemoveAsync(sessionCookie);
    }

    public DateTimeOffset SessionExpiration => DateTimeOffset.Now.Add(ExpirationTime);
}