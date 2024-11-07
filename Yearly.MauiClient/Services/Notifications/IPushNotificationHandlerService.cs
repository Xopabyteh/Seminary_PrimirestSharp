namespace Yearly.MauiClient.Services.Notifications;
public interface IPushNotificationHandlerService
{
    /// <summary>
    /// Id which this handler can handle. Provided by <see cref="Yearly.Contracts.Notifications.PushContracts"/>
    /// </summary>
    int NotificationId { get; }
    Task HandleNotificationAsync(IDictionary<string, string> data);
}
