using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Notifications;

public class AzureUserNotificationService : IUserNotificationService
{
    private readonly INotificationHubClient _client;

    public AzureUserNotificationService(INotificationHubClient client, IDistributedCache notificationTokensCache)
    {
        _client = client;
    }
    public Task SendPushNotificationAsync(
        string title,
        string message,
        UserId toUserId,
        NotificationTagContract notificationTag,
        IDictionary<string, object>? customData = null)
    {
        //Todo: deal with apple (IOS)
        return SendPushNotificationToAndroidAsync(title, message, toUserId, notificationTag, customData);
    }
    private Task SendPushNotificationToAndroidAsync(
        string title,
        string message,
        UserId toUserId,
        NotificationTagContract notificationTag,
        IDictionary<string, object>? customData = null)
    {
        // Sample:
        /*{
              "notification":{
                  "title":"Notification Hub Test Notification",
                  "body":"This is a sample notification delivered by Azure Notification Hubs."
          
              },
              "data":{
                  "property1":"value1",
                  "property2":42
          
              }
          }*/

        //Add notification id to data
        customData ??= new Dictionary<string, object>(1);
        customData.Add(NotificationDataKeysContract.k_NotificationIdKey, notificationTag.NotificationId);

        var payload = new
        {
            notification = new
            {
                title,
                body = message
            },
            data = customData
        };
        var payloadJson = JsonConvert.SerializeObject(payload);

        return _client.SendNotificationAsync(
            new FcmNotification(payloadJson),
            NotificationTagContract.AssembleUserSpecificTag(notificationTag, toUserId.Value));
    }
}