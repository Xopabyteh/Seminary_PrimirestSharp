namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

/// <summary>
/// This Id is from Primirest (MenuID) and is used to identify the menu for a week.
/// </summary>
public sealed class MenuForWeekId : ValueObject
{
    public int Value { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public MenuForWeekId(int value)
    {
        Value = value;
    }
}