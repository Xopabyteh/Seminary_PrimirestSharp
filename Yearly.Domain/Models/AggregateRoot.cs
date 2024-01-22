namespace Yearly.Domain.Models;

public class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : ValueObject
{
    private readonly List<IDomainEvent> _publishedEvents = new();

    protected AggregateRoot(TId id) 
        : base(id)
    {
    }

    private AggregateRoot() //For EF Core
    {
    }

    /// <summary>
    /// Publishes a domain event to the internal list of published events.
    /// </summary>
    /// <param name="dEvent"></param>
    protected void PublishDomainEvent(IDomainEvent dEvent)
        => _publishedEvents.Add(dEvent);

    /// <summary>
    /// A method that returns domain events published by the Entity, ValueObject or AggregateRoot.
    /// The list retrieved here shall not be cleared when <see cref="ClearDomainEvents"/> is called
    /// and shall return a copy (instance that won't mutate the interns) of the internally stored events of the object.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
        => _publishedEvents.ToList().AsReadOnly();

    /// <summary>
    /// Clears the internal list of domain events, while not in any way affecting the output of <see cref="GetDomainEvents"/>
    /// </summary>
    public void ClearDomainEvents()
        => _publishedEvents.Clear();
}