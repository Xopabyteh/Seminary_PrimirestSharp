using MediatR;
using Yearly.Domain.Models.UserAgg.DomainEvents;

namespace Yearly.Application.Users.DomainEvents;

internal sealed class NotifyPhotoApproversOnUserPublishedPhotoHandler : INotificationHandler<UserPublishedNewPhotoDomainEvent>
{
    public NotifyPhotoApproversOnUserPublishedPhotoHandler()
    {
    }

    public Task Handle(UserPublishedNewPhotoDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO:
        return Task.CompletedTask;
    }
}