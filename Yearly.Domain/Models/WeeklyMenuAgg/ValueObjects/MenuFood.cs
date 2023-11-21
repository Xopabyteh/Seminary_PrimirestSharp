using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class MenuFood : ValueObject
{
    public FoodId FoodId { get; private set; }

    public MenuFood(FoodId foodId)
    {
        FoodId = foodId;
    }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return FoodId;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MenuFood()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }

}