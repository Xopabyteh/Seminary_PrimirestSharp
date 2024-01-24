using MediatR;

namespace Yearly.Domain.Models;

#pragma warning disable SA1106 // Code should not contain empty statements
public interface IDomainEvent : INotification;
#pragma warning restore SA1106 // Code should not contain empty statements