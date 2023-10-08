namespace Yearly.Contracts.Menu;

public record MenuResponse(List<MenuObjResponse> Menus);

public record MenuObjResponse(
    DateTime Date, 
    List<FoodObjResponse> Foods
);

public record FoodObjResponse(
    string Name,
    string Allergens,
    List<string> Images);

