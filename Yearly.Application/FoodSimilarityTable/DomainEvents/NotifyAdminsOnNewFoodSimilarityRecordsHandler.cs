using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.FoodAgg.DomainEvents;

namespace Yearly.Application.FoodSimilarityTable.DomainEvents;

public class NotifyAdminsOnNewFoodSimilarityRecordsHandler : INotificationHandler<NewFoodSimilarityRecordsDomainEvent>
{
    private readonly IUserNotificationService _notificationService;

    public NotifyAdminsOnNewFoodSimilarityRecordsHandler(IUserNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task Handle(NewFoodSimilarityRecordsDomainEvent notification, CancellationToken cancellationToken)
    {
        return _notificationService.SendPushNotificationAsync(
            "Similarity",
            "New unresolved food similarities",
            NotificationTagContract.NewSimilarityRecord());
    }
}