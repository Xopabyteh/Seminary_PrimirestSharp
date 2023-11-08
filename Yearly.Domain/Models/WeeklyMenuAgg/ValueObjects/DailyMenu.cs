namespace Yearly.Domain.Models.MenuAgg.ValueObjects;

public sealed class DailyMenu : ValueObject
{
    private readonly List<Guid> _foodIds;
    public IReadOnlyList<Guid> FoodIds => _foodIds.AsReadOnly();

    public DateTime Date { get; private set; }

    public DailyMenu(
        List<Guid> foodIds,
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