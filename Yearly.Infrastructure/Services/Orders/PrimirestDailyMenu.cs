namespace Yearly.Infrastructure.Services.Orders;

public readonly record struct PrimirestDailyMenu(
    DateTime Date,
    List<PrimirestFood> Foods,
    PrimirestSoup Soup
    );