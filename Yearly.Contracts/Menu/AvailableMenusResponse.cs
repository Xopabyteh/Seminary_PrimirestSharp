namespace Yearly.Contracts.Menu;

public record AvailableMenusResponse(
    List<MenuResponse> Menus);

public record MenuResponse(
    DateTime Date, 
    List<FoodResponse> Foods);

public record FoodResponse(
    string Name,
    string Allergens,
    List<string> Images,
    PrimirestOrderIdentifierResponse PrimirestOrderIdentifier
    );

public record PrimirestOrderIdentifierResponse(
    int MenuId,
    int DayId,
    int ItemId);

