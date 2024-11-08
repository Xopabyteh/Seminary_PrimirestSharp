using System.Globalization;
using FirebaseAdmin.Messaging;
using MediatR;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.FoodAgg.DomainEvents;

namespace Yearly.Application.FoodSimilarityTable.DomainEvents;

public class NotifyAdminsOnNewFoodSimilarityRecordsHandler : INotificationHandler<NewFoodSimilarityRecordsDomainEvent>
{
    private readonly FirebaseMessaging _firebaseMessaging;

    public NotifyAdminsOnNewFoodSimilarityRecordsHandler(FirebaseMessaging firebaseMessaging)
    {
        _firebaseMessaging = firebaseMessaging;
    }

    public Task Handle(NewFoodSimilarityRecordsDomainEvent notification, CancellationToken cancellationToken)
    {
        var messageData = new Dictionary<string, string>()
        {
            [PushContracts.General.k_NotificationIdKey] = PushContracts.SimilarityTable.k_NewSimilarityRecordId.ToString(CultureInfo.InvariantCulture)
        };

        return _firebaseMessaging.SendAsync(
            new Message()
            {
                Data = messageData,
                Topic = PushContracts.SimilarityTable.NewSimilarityRecordTopic,
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