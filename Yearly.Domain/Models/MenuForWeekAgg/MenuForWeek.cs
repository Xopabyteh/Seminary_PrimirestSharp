﻿using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuForWeekAgg;

public class MenuForWeek : AggregateRoot<PrimirestMenuForWeekId>
{
    private List<MenuForDay> _menusForDays;
    public IReadOnlyList<MenuForDay> MenusForDays => _menusForDays;
    protected MenuForWeek(PrimirestMenuForWeekId id, List<MenuForDay> menusForDays) 
        : base(id)
    {
        _menusForDays = menusForDays;
    }

    public static MenuForWeek Create(PrimirestMenuForWeekId id ,List<MenuForDay> menusForDay)
    {
        return new(id, menusForDay);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MenuForWeek()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(null!)
    {
    }
}