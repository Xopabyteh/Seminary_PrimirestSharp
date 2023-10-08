namespace Yearly.Domain.Models.UserAgg.ValueObjects;

public class UserId : ValueObject
{
    public Guid Value { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}