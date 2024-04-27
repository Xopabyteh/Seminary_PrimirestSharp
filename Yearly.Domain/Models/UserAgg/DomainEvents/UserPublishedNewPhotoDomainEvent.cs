using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg.DomainEvents;

public record UserPublishedNewPhotoDomainEvent(UserId User, PhotoId PhotoId) : IDomainEvent;