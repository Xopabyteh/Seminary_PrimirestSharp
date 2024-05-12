using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Users.DomainEvents;

public class NotifyPublisherOnPhotoApprovedHandler : INotificationHandler<UserApprovedPhotoDomainEvent>
{
    private readonly IUserNotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IPhotoRepository _photoRepository;

    public NotifyPublisherOnPhotoApprovedHandler(IUserNotificationService notificationService, IPhotoRepository photoRepository, IUserRepository userRepository)
    {
        _notificationService = notificationService;
        _photoRepository = photoRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(UserApprovedPhotoDomainEvent request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);

        if (photo is null)
            return; // The photo could've been deleted before the event was handled

        var publisher = await _userRepository.GetByIdAsync(photo.PublisherId);
        if(publisher is null)
            return; // The publisher could've been deleted before the event was handled

        if (publisher.Roles.Contains(UserRole.PhotoApprover))
            return; // Don't notify photo approvers themselves

        await _notificationService.SendPushNotificationAsync(
            "Díky",
            "Vaše fotka byla schválena",
            NotificationTagContract.PhotoApproved(photo.PublisherId.Value));
    }
}