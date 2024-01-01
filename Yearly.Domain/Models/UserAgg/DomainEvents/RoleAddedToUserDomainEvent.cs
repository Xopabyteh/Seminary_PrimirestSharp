using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg.DomainEvents;

public record RoleAddedToUserDomainEvent(UserId toUser) : IDomainEvent;