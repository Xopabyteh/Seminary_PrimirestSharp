namespace Yearly.Application.Menus;

public readonly record struct ExternalServiceMenu(
    DateTime Date,
    List<ExternalServiceFood> Foods);