namespace Yearly.Application.Menus;

public readonly record struct PrimirestMenuForWeek(
    List<PrimirestMenuForDay> MenusForDay,
    int PrimirestMenuId // The id of the menu in Primirest, used for ordering. The one stored in the horrible selection field that has to be scraped
);
