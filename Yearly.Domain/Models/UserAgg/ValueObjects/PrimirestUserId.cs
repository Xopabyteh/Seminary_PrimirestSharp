namespace Yearly.Domain.Models.UserAgg.ValueObjects;

public class PrimirestUserId : ValueObject
{
    public int Value { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public PrimirestUserId(int value)
    {
        Value = value;
    }
}