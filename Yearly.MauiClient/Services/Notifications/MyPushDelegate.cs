using Plugin.LocalNotification;
using Shiny.Push;
using Yearly.Contracts.Notifications;

namespace Yearly.MauiClient.Services.Notifications;

public class MyPushDelegate : IPushDelegate
{
    public Task OnEntry(PushNotification push)
    {
        // fires when the user taps on a push notification
        //NOOP
        return Task.CompletedTask;
    }

    public async Task OnReceived(PushNotification push)
    {
        // fires when a push notification is received
        // iOS: set content-available: 1  or this won't fire
        // Android: Set data portion of payload

        var isIdPresent =
            push.Data.TryGetValue(NotificationDataKeysContract.k_NotificationIdKey, out var notificationIdStr);

        var notificationId = isIdPresent
            ? int.Parse(notificationIdStr!)
            : 1337; //Default notification id (shouldn't happen)
        
        //Show notification
        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = push.Notification?.Title!,
            Description = push.Notification?.Message!,
            Android =
            {
                IconSmallName =
                {
                    ResourceName = "notificationicon"
                }
            }
        };

        await LocalNotificationCenter.Current.Show(notification);
    }

    public Task OnNewToken(string token)
    {
        //NOOP
        return Task.CompletedTask;
    }

    public Task OnUnRegistered(string token)
    {
        //NOOP
        return Task.CompletedTask;
    }
}
