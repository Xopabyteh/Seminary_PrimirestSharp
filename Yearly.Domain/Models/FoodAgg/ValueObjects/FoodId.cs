namespace Yearly.Domain.Models.FoodAgg.ValueObjects;

public sealed class FoodId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public FoodId(Guid value)
    {
        Value = value;
    }
}