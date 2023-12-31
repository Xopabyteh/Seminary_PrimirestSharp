namespace Yearly.Infrastructure.Services.Orders.PrimirestStructures;

public readonly record struct PrimirestDailyMenu(
    DateTime Date,
    List<PrimirestFood> Foods,
    PrimirestSoup Soup
    );