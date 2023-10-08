namespace Yearly.Domain.Models.UserAgg.ValueObjects;

public class UserId : ValueObject
{
    public int Value { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public UserId(int value)
    {
        Value = value;
    }
}