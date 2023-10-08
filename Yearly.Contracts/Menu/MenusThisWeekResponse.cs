namespace Yearly.Contracts.Menu;

public record MenusThisWeekResponse(
    List<MenuResponse> Menus);

public record MenuResponse(
    DateTime Date, 
    List<FoodResponse> Foods);

public record FoodResponse(
    string Name,
    string Allergens,
    List<string> Images);

