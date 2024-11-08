using System.Globalization;
using FirebaseAdmin.Messaging;
using MediatR;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.PhotoAgg.DomainEvents;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Users.DomainEvents;

/// <summary>
/// Notifies approvers about new waiting photo
/// </summary>
internal sealed class NotifyPhotoApproversOnNewPhotoProcessed : INotificationHandler<PhotoThumbnailWasSet>
{
    private readonly FirebaseMessaging _firebaseMessaging;
    private readonly IPhotoRepository _photoRepository;

    public NotifyPhotoApproversOnNewPhotoProcessed(IPhotoRepository photoRepository, FirebaseMessaging firebaseMessaging)
    {
        _photoRepository = photoRepository;
        _firebaseMessaging = firebaseMessaging;
    }

    public async Task Handle(PhotoThumbnailWasSet request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);

        if (photo is null)
            return; // The photo could've been deleted before the event was handled

        if (photo.IsApproved)
            return; // Don't notify if the photo is already approved

        var messageData = new Dictionary<string, string>()
        {
            [PushContracts.General.k_NotificationIdKey] = PushContracts.Photos.k_NewWaitingPhotoNotificationId.ToString(CultureInfo.InvariantCulture)
        };

        await _firebaseMessaging.SendAsync(
            new Message()
            {
                Data = messageData,
                Topic = PushContracts.Photos.NewWaitingPhotoTopic,
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