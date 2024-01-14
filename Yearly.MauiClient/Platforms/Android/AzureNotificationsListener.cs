using Android.Content;
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

        

        //Log data
        foreach (var entity in data)
        {
            Android.Util.Log.Debug("AZURE-NOTIFICATION-HUBS", "Key: {0}, Value: {1}", entity.Key, entity.Value);
        }

        //Get notifications permissions
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            });
        }
        
        //Show notification
        var notification = new NotificationRequest
        {
            NotificationId = 1337,
            Title = message.Title,
            Description = message.Body,
            IconSmallName =
            {
                ResourceName = "notificationicon"
            }
        };
        await LocalNotificationCenter.Current.Show(notification);
    }
}