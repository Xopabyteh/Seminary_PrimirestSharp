namespace Yearly.Domain.Models.Common.ValueObjects;

public class MoneyCzechCrowns : ValueObject
{
    public decimal Value { get; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public MoneyCzechCrowns(decimal value)
    {
        Value = value;
    }
}