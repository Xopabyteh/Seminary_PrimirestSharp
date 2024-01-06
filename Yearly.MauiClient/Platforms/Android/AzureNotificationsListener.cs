using Android.Content;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.LocalNotification;
using WindowsAzure.Messaging.NotificationHubs;

namespace Yearly.MauiClient;

internal class AzureNotificationsListener : Java.Lang.Object, INotificationListener
{
    internal const string HubName = "PrimirestSharpNH";
    internal const string ListenConnectionString = "Endpoint=sb://PrimirestSharpNotificationHubNS.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=jbGQazmRlrCHqKOyIO+/UoA5YeY0PcfCiKx0xqBA6Ys=";

    public async void OnPushNotificationReceived(Context context, INotificationMessage message)
    {
        //Push Notification arrived - print out the keys/values
        var data = message.Data;
        if (data == null)
            return;

        foreach (var entity in data)
        {
            Android.Util.Log.Debug("AZURE-NOTIFICATION-HUBS", "Key: {0}, Value: {1}", entity.Key, entity.Value);
        }

        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            });
        }

        var notification = new NotificationRequest
        {
            NotificationId = 100,
            Title = "Test",
            Description = "Test Description",
            ReturningData = "Dummy data", // Returning data when tapped on notification.
            Schedule =
            {
                NotifyTime = DateTime.Now.AddSeconds(5) // Used for Scheduling local notification, if not specified notification will show immediately.
            }
        };
        await LocalNotificationCenter.Current.Show(notification);
    }
}