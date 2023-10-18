namespace Yearly.Domain.Models;

public class AggregateRoot<TId> : Entity<TId>
    where TId : ValueObject
{
    protected AggregateRoot(TId id) 
        : base(id)
    {
    }

    private AggregateRoot() //For EF Core
    {
    }
}