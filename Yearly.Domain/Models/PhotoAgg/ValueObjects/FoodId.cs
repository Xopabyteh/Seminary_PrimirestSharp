namespace Yearly.Domain.Models.PhotoAgg.ValueObjects;

public sealed class PhotoId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public PhotoId(Guid value)
    {
        Value = value;
    }

    private PhotoId() // For EF Core
    {
        
    }
}