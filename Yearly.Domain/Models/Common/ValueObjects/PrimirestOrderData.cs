namespace Yearly.Domain.Models.Common.ValueObjects;

public class PrimirestOrderData : ValueObject
{
    public PrimirestFoodOrderIdentifier PrimirestFoodOrderIdentifier { get; }
    public MoneyCzechCrowns Money { get;}
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return PrimirestFoodOrderIdentifier;
        yield return Money;
    }

    public PrimirestOrderData(PrimirestFoodOrderIdentifier primirestFoodOrderIdentifier, MoneyCzechCrowns money)
    {
        PrimirestFoodOrderIdentifier = primirestFoodOrderIdentifier;
        Money = money;
    }
}