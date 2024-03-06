using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Common.Interfaces;

public interface IUserNotificationService
{
    public Task SendPushNotificationAsync(
        string title,
        string message,
        UserId toUserId,
        NotificationTagContract notificationTag,
        IDictionary<string, object>? customData = null);
}