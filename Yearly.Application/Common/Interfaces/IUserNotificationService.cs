using Yearly.Contracts.Notifications;

namespace Yearly.Application.Common.Interfaces;

public interface IUserNotificationService
{
    public Task SendPushNotificationAsync(
        string title,
        string message,
        NotificationTagContract notificationTag,
        IDictionary<string, object>? customData = null);
}