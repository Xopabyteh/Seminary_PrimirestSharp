namespace Yearly.Application.Menus;

public readonly record struct PrimirestDailyMenu(
    DateTime Date,
    List<PrimirestFood> Foods,
    PrimirestSoup Soup
    );