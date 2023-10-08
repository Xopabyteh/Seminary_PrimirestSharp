namespace Yearly.Domain.Models.PhotoAgg.ValueObjects;

public class PhotoId : ValueObject
{
    public Guid Value { get; private set; }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}