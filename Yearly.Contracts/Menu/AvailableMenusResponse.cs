using Yearly.Contracts.Common;

namespace Yearly.Contracts.Menu;

public record AvailableMenusResponse(List<WeeklyMenuResponse> WeeklyMenus);

public record WeeklyMenuResponse(
    List<DailyMenuResponse> DailyMenus,
    int PrimirestMenuId);
public record DailyMenuResponse(
    DateTime Date, 
    List<FoodResponse> Foods
    //SoupResponse Soup
    );

public record FoodResponse(
    string Name,
    string Allergens,
    List<string> PhotoLinks,
    Guid FoodId, //The id of the food in our system, not the primirest id
    PrimirestFoodIdentifierContract PrimirestFoodIdentifier
    );

//public record SoupResponse(
//    string Name,
//    List<string> ImageLinks);
