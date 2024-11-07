using Plugin.LocalNotification;
using Yearly.Contracts.Notifications;

namespace Yearly.MauiClient.Services.Notifications.Photos;
public class PhotoApprovedNotificationHandler : IPushNotificationHandlerService
{
    public int NotificationId => PushContracts.Photos.k_PhotoApprovedNotificationId;

    public async Task HandleNotificationAsync(IDictionary<string, string> data)
    {
        await LocalNotificationCenter.Current.Show(new()
        {
            NotificationId = NotificationId,
            Title = "Fotka schválena",
            Description = "Díky, pomáháš všem <3",
            Android = new()
            {
                ChannelId = PushContracts.General.k_GeneralNotificationChannelId,
                IconSmallName = new()
                {
                    ResourceName = "notificationicon"
                }
            },
        });
    }
}
