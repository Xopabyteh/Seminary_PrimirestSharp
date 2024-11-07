using Plugin.LocalNotification;
using Yearly.Contracts.Notifications;

namespace Yearly.MauiClient.Services.Notifications.Photos;
public class NewWaitingPhotoNotificationHandler : IPushNotificationHandlerService
{
    public int NotificationId => PushContracts.Photos.k_NewWaitingPhotoNotificationId;
    public async Task HandleNotificationAsync(IDictionary<string, string> data)
    {
        await LocalNotificationCenter.Current.Show(new()
        {
            NotificationId = NotificationId,
            Title = "Nová fotka",
            Description = "Někdo nahrál novou fotku, kterou je třeba schválit.",
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
