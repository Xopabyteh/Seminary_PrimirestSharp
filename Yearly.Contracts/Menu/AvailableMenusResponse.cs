using Yearly.Contracts.Common;

namespace Yearly.Contracts.Menu;

public record AvailableMenusResponse(List<MenuForWeekResponse> MenusForWeeks);

public record MenuForWeekResponse(
    List<MenuForDayResponse> MenusForDay,
    int PrimirestMenuId);
public record MenuForDayResponse(
    DateTime Date, 
    List<FoodResponse> Foods
    //SoupResponse Soup
    );

public record FoodResponse(
    string Name,
    string Allergens,
    List<string> PhotoLinks,
    PrimirestFoodIdentifierResponse PrimirestFoodIdentifier
    );

//public record SoupResponse(
//    string Name,
//    List<string> ImageLinks);
