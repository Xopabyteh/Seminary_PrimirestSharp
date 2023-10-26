using Yearly.Contracts.Common;

namespace Yearly.Contracts.Menu;

public record AvailableMenusResponse(
    List<MenuForDayResponse> Menus);

public record MenuForDayResponse(
    DateTime Date, 
    List<FoodResponse> Foods
    //SoupResponse Soup
    );

public record FoodResponse(
    string Name,
    string Allergens,
    List<string> ImageLinks,
    PrimirestOrderIdentifierResponse PrimirestOrderIdentifier
    );

//public record SoupResponse(
//    string Name,
//    List<string> ImageLinks);
