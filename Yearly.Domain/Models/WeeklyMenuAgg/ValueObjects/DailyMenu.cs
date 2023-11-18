using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class DailyMenu : ValueObject
{
    //private readonly List<Guid> _foodIds;
    //public IReadOnlyList<Guid> FoodIds => _foodIds.AsReadOnly();
    private readonly List<Food> _foods;
    public IReadOnlyList<Food> Foods => _foods.AsReadOnly();

    public DateTime Date { get; private set; }

    public DailyMenu(
        List<Food> foods,
        DateTime date)
    {
        _foods = foods;
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
        foreach (var food in _foods)
        {
            yield return food;
        }
    }
}