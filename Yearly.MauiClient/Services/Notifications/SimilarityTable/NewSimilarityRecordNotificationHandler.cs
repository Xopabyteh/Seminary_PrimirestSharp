using Plugin.LocalNotification;
using Yearly.Contracts.Notifications;

namespace Yearly.MauiClient.Services.Notifications.SimilarityTable;
public class NewSimilarityRecordNotificationHandler : IPushNotificationHandlerService
{
    public int NotificationId => PushContracts.SimilarityTable.k_NewSimilarityRecordId;

    public async Task HandleNotificationAsync(IDictionary<string, string> data)
    {
        await LocalNotificationCenter.Current.Show(new()
        {
            NotificationId = NotificationId,
            Title = "Nový záznam podobnosti",
            Description = "Byl přidán nový záznam podobnosti.",
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
