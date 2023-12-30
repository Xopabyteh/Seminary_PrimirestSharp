namespace Yearly.Infrastructure.Services.Orders;

public readonly record struct PrimirestWeeklyMenu(
    List<PrimirestDailyMenu> DailyMenus,
    int PrimirestMenuId // The id of the menu in Primirest, used for ordering. The one stored in the horrible selection field that has to be scraped
);
