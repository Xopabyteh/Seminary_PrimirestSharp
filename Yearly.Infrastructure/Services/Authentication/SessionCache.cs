using System.Reflection;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Authentication;

public class SessionCache : ISessionCache
{ 
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30);

    private readonly IDistributedCache _userCache;
    private readonly IUnitOfWork _unitOfWork;

    public SessionCache(IDistributedCache userCache, IUnitOfWork unitOfWork)
    {
        _userCache = userCache;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(string sessionCookie, User user)
    {
        var userJson = JsonConvert.SerializeObject(user);

        var options = new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = ExpirationTime};
        await _userCache.SetStringAsync(sessionCookie, userJson, options);
    }

    public async Task<User?> GetAsync(string sessionCookie)
    {
        var userJson = await _userCache.GetStringAsync(sessionCookie);
        if (string.IsNullOrEmpty(userJson))
            return null;

        var user = JsonConvert.DeserializeObject<User>(userJson, new UserDeserializerConverter())!;
        //_unitOfWork.AddForUpdate(user);
        
        return user;
    }

    public Task RemoveAsync(string sessionCookie)
    {
        return _userCache.RemoveAsync(sessionCookie);
    }

    public DateTimeOffset SessionExpiration => DateTimeOffset.Now.Add(ExpirationTime);

    private class UserDeserializerConverter : JsonConverter<User>
    {
        public override void WriteJson(JsonWriter writer, User? value, JsonSerializer serializer)
        {
            throw new NotImplementedException("This custom converter only supports deserialization.");
        }

        public override User? ReadJson(
            JsonReader reader,
            Type objectType, 
            User? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            // Load JSON into JObject
            var jsonObject = JObject.Load(reader);
            var json = jsonObject.ToString();

            // Extract properties from JSON
            var userIdValue = jsonObject[nameof(User.Id)]![nameof(User.Id.Value)]!.Value<int>()!;
            var username = jsonObject[nameof(User.Username)]!.Value<string>()!;
            var roles = jsonObject[nameof(User.Roles)]!.ToObject<List<UserRole>>()!;
            var photoIds = jsonObject[nameof(User.PhotoIds)]!.ToObject<List<PhotoId>>()!;

            // Create a new User object with reflection
            var user = new User(new UserId(userIdValue), username);
            SetBackingFieldByConvention(nameof(User.Roles), user, roles);
            SetBackingFieldByConvention(nameof(User.PhotoIds), user, photoIds);

            return user;
        }

        private void SetBackingFieldByConvention<TObj>(string propertyName, TObj ofObject, object toValue)
        {
            //Convention obviously being
            //PropertyName ->
            //_propertyName
            var propertyNameSpan = propertyName.AsSpan();
            var backingFieldName = new StringBuilder(propertyName.Length + 1);
            backingFieldName.Append('_');
            backingFieldName.Append(char.ToLower(propertyNameSpan[0]));
            backingFieldName.Append(propertyNameSpan.Slice(1));

            var backingField = typeof(TObj).GetField(
                backingFieldName.ToString(),
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (backingField is null)
                throw new InvalidOperationException($"Backing field for property {propertyName} not found on type {typeof(TObj).Name}");

            backingField.SetValue(ofObject, toValue);
        }
    }
}