using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class DailyMenu : ValueObject
{
    private readonly List<MenuFood> _foods;
    public IReadOnlyList<MenuFood> Foods => _foods.AsReadOnly();

    public DateTime Date { get; private set; }

    public DailyMenu(
        List<FoodId> foods,
        DateTime date)
    {
        _foods = foods.ConvertAll(f => new MenuFood(f));
        Date = date;
    }

#pragma warning disable CS8618 //For EF Core
    // ReSharper disable once UnusedMember.Local
    private DailyMenu()
#pragma warning restore CS8618
    {

    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        foreach (var foodId in _foods)
        {
            yield return foodId;
        }
    }
}