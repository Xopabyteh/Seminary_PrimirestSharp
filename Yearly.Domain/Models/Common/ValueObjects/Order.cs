using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Models.Common.ValueObjects;
public class Order : ValueObject
{
    public FoodId ForFood { get; }
    public PrimirestOrderData PrimirestOrderData { get; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ForFood;
        yield return PrimirestOrderData;
    }

    public Order(FoodId forFood, PrimirestOrderData primirestOrderData)
    {
        ForFood = forFood;
        PrimirestOrderData = primirestOrderData;
    }
}