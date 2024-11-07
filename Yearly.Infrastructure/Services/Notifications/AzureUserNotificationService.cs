using System.Globalization;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;

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
        NotificationTagContract notificationTag,
        IDictionary<string, object>? customData = null)
    {
        // Todo: deal with IOS

        return SendPushNotificationToAndroidAsync(
            title,
            message,
            notificationTag.Value,
            notificationTag.NotificationId,
            customData);
    }

    /// <summary>
    /// Converts a dictionary, with quotation marks around numbers
    /// </summary>
    private static string DataToFcmV1Json(IDictionary<string, object> data)
    {
        // TODO: maybe not the best way to do it? allocating a whole extra dictionary just for this?
        object NumberToStringOrValue(object value)
            => value switch
            {
                int intValue => intValue.ToString(),
                long longValue => longValue.ToString(),
                float floatValue => floatValue.ToString(CultureInfo.InvariantCulture),
                double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture),
                decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),
                _ => value
            };

        // We only have to put quotation marks around numbers
        var dataJson = JsonConvert.SerializeObject(
            data.Select(p =>
                    new KeyValuePair<string, object>(p.Key, NumberToStringOrValue(p.Value)))
                .ToDictionary());

        return dataJson;
    }

    /// <summary>
    /// Note: this isn't safe and anyone can listen to this notification.
    /// In case of sending sensitive content, something more secure must be created. 
    /// </summary>
    private Task SendPushNotificationToAndroidAsync(
        string title,
        string message,
        string notificationTag,
        int notificationId,
        IDictionary<string, object>? customData = null)
    {
        // Sample:
        /*
                {
                  "message": {
                    "notification": {
                      "title": "Breaking News",
                      "body": "New news story available."
                    },
                    "data": {
                      "notification-id": "13"
                    }
                  }
                }
          */

        //Add notification id to data
        customData ??= new Dictionary<string, object>(1);
        customData.Add(PushContracts.k_NotificationIdKey, notificationId);

        var payloadJson = $$"""
                            {
                              "message": {
                                "notification": {
                                  "title": "{{title}}",
                                  "body": "{{message}}"
                                },
                                "data": {{DataToFcmV1Json(customData)}}
                              }
                            }
                            """;

        return _client.SendNotificationAsync(
            new FcmV1Notification(payloadJson),
            notificationTag);
    }
}