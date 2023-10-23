namespace Yearly.Domain.Models.FoodAgg.ValueObjects;

/// <summary>
/// We use this to identify the foods when ordering them via Primirest
/// </summary>
public class PrimirestOrderIdentifier : ValueObject
{
    public int MenuId { get; }
    public int DayId  {get; }
    public int ItemId { get; }

    public PrimirestOrderIdentifier(int menuId, int dayId, int itemId)
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


    private PrimirestOrderIdentifier() // For EF Core
    {
    }
}
