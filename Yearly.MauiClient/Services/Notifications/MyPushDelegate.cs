using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Shiny.Push;
using Yearly.Contracts.Notifications;

namespace Yearly.MauiClient.Services.Notifications;

public class MyPushDelegate : PushDelegate
{
    public override async Task OnReceived(PushNotification push)
    {
        // fires when a push notification is received
        // iOS: set content-available: 1  or this won't fire
        // Android: Set data portion of payload

        // Log
        Console.WriteLine("Push Notification Received: " + push.Notification?.Message);

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
            #if ANDROID
            Android =
                new AndroidOptions {
                    ChannelId = "server_notifications",
                    IconSmallName = new AndroidIcon(nameof(_Microsoft.Android.Resource.Designer.Resource.Drawable.notificationicon)),
                    IconLargeName = new AndroidIcon(nameof(_Microsoft.Android.Resource.Designer.Resource.Drawable.notificationicon))
                }
            #endif
        };

        await LocalNotificationCenter.Current.Show(notification);
    }
}
