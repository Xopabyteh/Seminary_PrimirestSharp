using Yearly.Contracts.Notifications;

namespace Yearly.MauiClient.Services.Notifications;

public class PushNotificationHandlerService
{
    public const string k_FCMTokenPrefKey = nameof(k_FCMTokenPrefKey);

    private IEnumerable<IPushNotificationHandlerService> _handlers;

    public PushNotificationHandlerService(IEnumerable<IPushNotificationHandlerService> handlers)
    {
        _handlers = handlers;
    }

    /// <summary>
    /// Finds appropriate handler for the notificationId and handles the notification
    /// </summary>
    public async Task HandleNotificationAsync(IDictionary<string, string> data)
    {
        var id = ExtractId(data);
        if (id is null)
        {
            Console.WriteLine("P# FCM Couldn't extract id from notification data, dropping request.");
        }

        var handler = _handlers.FirstOrDefault(h => h.NotificationId == id!.Value);
        if (handler is null)
        {
            Console.WriteLine($"P# FCM Couldn't find handler for id {id}.");
            return;
        }

        Console.WriteLine($"Magy FCM Using {handler}");
        await handler.HandleNotificationAsync(data);
    }

    private int? ExtractId(IDictionary<string, string> data)
    {
        if (!data.TryGetValue(PushContracts.General.k_NotificationIdKey, out var idStr))
            return null;

        if (!int.TryParse(idStr, out var id))
            return null;

        return id;
    }
}