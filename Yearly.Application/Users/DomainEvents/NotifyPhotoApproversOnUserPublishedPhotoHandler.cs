using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Users.DomainEvents;

internal sealed class NotifyPhotoApproversOnUserPublishedPhotoHandler : INotificationHandler<UserPublishedNewPhotoDomainEvent>
{
    private readonly IUserNotificationService _notificationService;
    private readonly IPhotoRepository _photoRepository;

    public NotifyPhotoApproversOnUserPublishedPhotoHandler(IUserNotificationService notificationService, IPhotoRepository photoRepository)
    {
        _notificationService = notificationService;
        _photoRepository = photoRepository;
    }

    public async Task Handle(UserPublishedNewPhotoDomainEvent request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);

        if (photo is null)
            return; // The photo could've been deleted before the event was handled

        if (photo.IsApproved)
            return; // Don't notify if the photo is already approved

        await _notificationService.SendPushNotificationAsync(
            "Nová fotka",
            "Nová fotka čeká na schválení",
            NotificationTagContract.NewWaitingPhoto());
    }
}