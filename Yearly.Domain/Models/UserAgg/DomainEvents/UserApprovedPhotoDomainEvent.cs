using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg.DomainEvents;

public sealed record UserApprovedPhotoDomainEvent(
    UserId ApproverId,
    PhotoId PhotoId) 
    : IDomainEvent;
