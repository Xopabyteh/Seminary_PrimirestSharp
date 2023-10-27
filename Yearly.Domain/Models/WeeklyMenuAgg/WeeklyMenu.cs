using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Domain.Models.WeeklyMenuAgg;

public class WeeklyMenu : AggregateRoot<WeeklyMenuId>
{
    private List<DailyMenu> _dailyMenus;
    public IReadOnlyList<DailyMenu> DailyMenus => _dailyMenus;
    protected WeeklyMenu(WeeklyMenuId id, List<DailyMenu> dailyMenus) 
        : base(id)
    {
        _dailyMenus = dailyMenus;
    }

    public static WeeklyMenu Create(WeeklyMenuId id ,List<DailyMenu> dailyMenus)
    {
        return new(id, dailyMenus);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private WeeklyMenu()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(null!)
    {
    }
}