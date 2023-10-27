using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class MenuForDay : ValueObject
{
    private readonly List<FoodId> _foodIds;
    public IReadOnlyList<FoodId> FoodIds => _foodIds.AsReadOnly();

    public DateTime Date { get; private set; }

    public MenuForDay(
        List<FoodId> foodIds,
        DateTime date)
    {
        _foodIds = foodIds;
        Date = date;
    }

#pragma warning disable CS8618 //For EF Core
    private MenuForDay()
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