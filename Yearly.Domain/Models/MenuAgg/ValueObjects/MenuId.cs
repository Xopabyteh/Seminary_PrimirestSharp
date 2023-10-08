namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public class MenuId : ValueObject
{
    public Guid Value { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}