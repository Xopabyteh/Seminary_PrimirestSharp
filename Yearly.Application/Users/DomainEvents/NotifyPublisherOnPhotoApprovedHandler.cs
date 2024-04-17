using MediatR;
using Yearly.Domain.Models.UserAgg.DomainEvents;

namespace Yearly.Application.Users.DomainEvents;

public class NotifyPublisherOnPhotoApprovedHandler : INotificationHandler<UserApprovedPhotoDomainEvent>
{
    public Task Handle(UserApprovedPhotoDomainEvent notification, CancellationToken cancellationToken)
    {
        // Todo:

        return Task.CompletedTask;
    }
}