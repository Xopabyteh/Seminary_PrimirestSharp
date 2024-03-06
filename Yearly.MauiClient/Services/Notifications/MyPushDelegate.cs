using Plugin.LocalNotification;
using Shiny.Push;

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

        //Show notification
        var notification = new NotificationRequest
        {
            NotificationId = 1337,
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

    public Task OnTokenRefreshed(string token)
    {
        //NOOP
        return Task.CompletedTask;
    }
}
