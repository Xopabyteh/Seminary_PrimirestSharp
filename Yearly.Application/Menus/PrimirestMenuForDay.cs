namespace Yearly.Application.Menus;

public readonly record struct PrimirestMenuForDay(
    DateTime Date,
    List<PrimirestFood> Foods
    );