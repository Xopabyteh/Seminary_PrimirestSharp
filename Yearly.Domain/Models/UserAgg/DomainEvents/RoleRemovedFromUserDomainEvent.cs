using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg.DomainEvents;

public record RoleRemovedFromUserDomainEvent(UserId fromUser) : IDomainEvent;