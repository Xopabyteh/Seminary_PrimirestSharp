namespace Yearly.Domain.Models.Common.ValueObjects;

public class PrimirestOrderData : ValueObject
{
    public PrimirestFoodOrderIdentifier PrimirestFoodOrderIdentifier { get; }
    public decimal PriceCzechCrowns { get;}
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return PrimirestFoodOrderIdentifier;
        yield return PriceCzechCrowns;
    }

    public PrimirestOrderData(PrimirestFoodOrderIdentifier primirestFoodOrderIdentifier, decimal priceCzechCrowns)
    {
        PrimirestFoodOrderIdentifier = primirestFoodOrderIdentifier;
        PriceCzechCrowns = priceCzechCrowns;
    }
}