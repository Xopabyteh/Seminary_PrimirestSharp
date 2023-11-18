namespace Yearly.Domain.Models;

public abstract class AggregateRootId<TIdType> : ValueObject
{
    public abstract TIdType Value { get; protected set; }

#pragma warning disable CS8618 //For EF Core
    protected AggregateRootId()
    {
    }
#pragma warning restore CS8618
}