using Yearly.Contracts.Common;

namespace Yearly.Contracts.Menu;

public record AvailableMenusResponse(List<WeeklyMenuDTO> WeeklyMenus);

public record WeeklyMenuDTO(
    List<DailyMenuDTO> DailyMenus,
    int PrimirestMenuId);
public record DailyMenuDTO(
    DateTime Date, 
    List<FoodDTO> Foods
    //SoupResponse Soup
    );

//public record SoupResponse(
//    string Name,
//    List<string> ImageLinks);
