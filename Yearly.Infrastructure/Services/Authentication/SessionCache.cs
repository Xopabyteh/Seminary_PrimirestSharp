using Microsoft.Extensions.Caching.Memory;
using Yearly.Application.Authentication;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Authentication;

public class SessionCache : ISessionCache
{
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30);
    private readonly IDateTimeProvider _dateTimeProvider;

    //Key: Session cookie, Value: User
    private readonly IMemoryCache _cache;

    //Key: UserId, Value: Session Cookie
    private readonly Dictionary<UserId, CacheRecord> _userToCacheRecord; //Todo: what if _cache expires, but this does not


    public SessionCache(IMemoryCache cache, IDateTimeProvider dateTimeProvider)
    {
        _cache = cache;
        _dateTimeProvider = dateTimeProvider;
        _userToCacheRecord = new();
    }

    //Todo: this is 🤢
    public void Add(string sessionCookie, User user)
    {
        var now = _dateTimeProvider.UtcNow;

        if (_userToCacheRecord.TryGetValue(user.Id, out var cacheRecord))
        {
            //Remove old session and set new one

            _userToCacheRecord[user.Id] = new CacheRecord(sessionCookie, now); //Rewrite to newer session cookie
            Remove(cacheRecord.SessionCookie); //Remove old one
            _cache.Set(sessionCookie, user, ExpirationTime); //Create new one
            return;
        }

        //Cache new session
        _cache.Set(sessionCookie, user, ExpirationTime);
        _userToCacheRecord.Add(user.Id, new(sessionCookie, now));
    }

    public User? Get(string sessionCookie)
    {
        return _cache.Get<User>(sessionCookie);
    }

    public void Remove(string sessionCookie)
    {
        var user = Get(sessionCookie);
        if (user is null)
            return;

        _userToCacheRecord.Remove(user.Id);
        _cache.Remove(sessionCookie);
    }

    /// <summary>
    /// Gets user by id, if he's in cache, change his data to <see cref="newUserData"/>.
    /// </summary>
    /// <param name="newUserData"></param>
    public void InvalidateCache(User newUserData)
    {
        if (!_userToCacheRecord.TryGetValue(newUserData.Id, out var cacheRecord))
            return; //We never cached this user

        // Is the user in the session cache as well?
        if(!_cache.TryGetValue(cacheRecord.SessionCookie, out _))
        {
            //Users cache expired
            //Lazily remove this one aswell
            _userToCacheRecord.Remove(newUserData.Id);
            return;
        }

        //Update cache
        //_cache.CreateEntry(cacheRecord.SessionCookie)
        //    .SetValue(newUserData)
        //    .SetAbsoluteExpiration(cacheRecord.CacheEnteredTime.Add(ExpirationTime));
        _cache.Set(cacheRecord.SessionCookie, newUserData, cacheRecord.CacheEnteredTime.Add(ExpirationTime));
    }

    private readonly record struct CacheRecord(string SessionCookie, DateTimeOffset CacheEnteredTime);
}