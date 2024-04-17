using MediatR;
using Yearly.Domain.Models.FoodAgg.DomainEvents;

namespace Yearly.Application.Foods.DomainEvents;

public class NotifyAdminsOnNewFoodSimilarityRecordsHandler : INotificationHandler<NewFoodSimilarityRecordsDomainEvent>
{
    public Task Handle(NewFoodSimilarityRecordsDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO:

        return Task.CompletedTask;
    }
}