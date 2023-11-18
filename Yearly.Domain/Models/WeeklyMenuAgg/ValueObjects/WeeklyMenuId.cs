namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

/// <summary>
/// This Id is from Primirest (MenuID) and is used to identify the menu for a week.
/// </summary>
public sealed class WeeklyMenuId : AggregateRootId<int>
{
    public override int Value { get; protected set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public WeeklyMenuId(int value)
    {
        Value = value;
    }
}