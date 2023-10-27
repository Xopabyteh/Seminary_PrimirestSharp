namespace Yearly.Domain.Models.UserAgg.ValueObjects;

/// <summary>
/// This Id is from Primirest (UserID) and is used to identify the user in their auth system.
/// </summary>
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