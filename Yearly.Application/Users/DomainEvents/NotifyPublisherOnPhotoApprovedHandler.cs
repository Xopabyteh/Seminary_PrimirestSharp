using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Users.DomainEvents;

public class NotifyPublisherOnPhotoApprovedHandler : INotificationHandler<UserApprovedPhotoDomainEvent>
{
    private readonly IUserNotificationService _notificationService;
    private readonly IPhotoRepository _photoRepository;

    public NotifyPublisherOnPhotoApprovedHandler(IUserNotificationService notificationService, IPhotoRepository photoRepository)
    {
        _notificationService = notificationService;
        _photoRepository = photoRepository;
    }

    public async Task Handle(UserApprovedPhotoDomainEvent request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);
        
        if (photo is null)
            return; // The photo could've been deleted before the event was handled

        await _notificationService.SendPushNotificationAsync(
            "Úspěch",
            "Vaše fotka byla schválena",
            photo.PublisherId,
            NotificationTagContract.PhotoApproved);
    }
}