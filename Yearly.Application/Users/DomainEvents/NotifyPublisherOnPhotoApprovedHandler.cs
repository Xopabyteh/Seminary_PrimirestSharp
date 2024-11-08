using System.Globalization;
using FirebaseAdmin.Messaging;
using MediatR;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Users.DomainEvents;

public class NotifyPublisherOnPhotoApprovedHandler : INotificationHandler<UserApprovedPhotoDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly FirebaseMessaging _firebaseMessaging;

    public NotifyPublisherOnPhotoApprovedHandler(
        IPhotoRepository photoRepository,
        IUserRepository userRepository,
        FirebaseMessaging firebaseMessaging)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _firebaseMessaging = firebaseMessaging;
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

        var messageData = new Dictionary<string, string>()
        {
            [PushContracts.General.k_NotificationIdKey] = PushContracts.Photos.k_PhotoApprovedNotificationId.ToString(CultureInfo.InvariantCulture)
        };

        var topicForPublisher = PushContracts.Photos.CreatePhotoApprovedTopic(publisher.Id.Value);
        await _firebaseMessaging.SendAsync(
            new Message()
            {
                Data = messageData,
                Topic = topicForPublisher,
                Apns = new()
                {
                    Aps = new()
                    {
                        ContentAvailable = true
                    }
                }
            },
            cancellationToken);
    }
}