using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class DailyMenu : ValueObject
{
    private readonly List<FoodId> _foodIds;
    public IReadOnlyList<FoodId> FoodIds => _foodIds.AsReadOnly();

    public DateTime Date { get; private set; }

    public DailyMenu(
        List<FoodId> foodIds,
        DateTime date)
    {
        _foodIds = foodIds;
        Date = date;
    }

#pragma warning disable CS8618 //For EF Core
    private DailyMenu()
#pragma warning restore CS8618
    {

    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        foreach (var foodId in _foodIds)
        {
            yield return foodId;
        }
    }
}