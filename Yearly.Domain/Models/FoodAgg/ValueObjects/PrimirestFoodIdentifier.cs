namespace Yearly.Domain.Models.FoodAgg.ValueObjects;

/// <summary>
/// We use this to identify the foods when ordering them via Primirest
/// </summary>
public class PrimirestFoodIdentifier : ValueObject
{
    public int MenuId { get; private set; }
    public int DayId  {get; private set; }
    public int ItemId { get; private set; }

    public PrimirestFoodIdentifier(int menuId, int dayId, int itemId)
    {
        MenuId = menuId;
        DayId = dayId;
        ItemId = itemId;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return MenuId;
        yield return DayId;
        yield return ItemId;
    }


    private PrimirestFoodIdentifier() // For EF Core
    {
    }
}
