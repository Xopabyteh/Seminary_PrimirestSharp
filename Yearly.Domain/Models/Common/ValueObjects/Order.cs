using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Models.Common.ValueObjects;
public class Order : ValueObject
{
    public FoodId ForFood { get; }
    public PrimirestFoodOrderIdentifier PrimirestOrderIdentifier { get; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ForFood;
        yield return PrimirestOrderIdentifier;
    }

    public Order(FoodId forFood, PrimirestFoodOrderIdentifier primirestOrderIdentifier)
    {
        ForFood = forFood;
        PrimirestOrderIdentifier = primirestOrderIdentifier;
    }
}
