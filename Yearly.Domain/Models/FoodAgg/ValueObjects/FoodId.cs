namespace Yearly.Domain.Models.FoodAgg.ValueObjects;

public class FoodId : ValueObject
{
    public Guid Value { get; private set; }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public FoodId(Guid value)
    {
        Value = value;
    }
}