namespace Yearly.Domain.Models;

public interface IDomainEventPublisher
{
    //public void PublishDomainEvent(IDomainEvent dEvent);

    /// <summary>
    /// A method that returns domain events published by the Entity, ValueObject or AggregateRoot.
    /// The list retrieved here shall not be cleared when <see cref="ClearDomainEvents"/> is called
    /// and shall return a copy of the internally stored events of the object.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears the internal list of domain events, while not in any way affecting the output of <see cref="GetDomainEvents"/>
    /// </summary>
    public void ClearDomainEvents();
}