namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class PrimirestMenuForWeekId : ValueObject
{
    public int Value { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public PrimirestMenuForWeekId(int value)
    {
        Value = value;
    }
}