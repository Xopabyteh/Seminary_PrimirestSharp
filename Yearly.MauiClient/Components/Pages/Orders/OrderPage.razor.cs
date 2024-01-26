using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class OrderPage
{
    [Inject] private MenuAndOrderCacheService MenuAndOrderCacheService { get; set; } = null!;

    private IReadOnlyList<WeeklyMenuDTO> weeklyMenus = Array.Empty<WeeklyMenuDTO>();
    private bool weeklyMenuDTOsLoaded = false; //Not the components themselves, but the DTOs.
                                               //Also implies that selected weeklyMenu and dailyMenu are loaded
    private WeeklyMenuDTO selectedWeeklyMenu;
    private DailyMenuDTO selectedDailyMenu;

    private Dictionary<WeeklyMenuDTO, WeeklyMenuSelectableVM> weeklyMenuSelectables = new(3);
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        //Get weekly menus
        weeklyMenus = await MenuAndOrderCacheService.CachedMenusAsync();
        foreach (var weeklyMenuDTO in weeklyMenus) //This only happens max like 5x, so it's chill on performance
        {
            var menuStartDate = weeklyMenuDTO.DailyMenus.Min(dm => dm.Date);
            var menuEndDate = weeklyMenuDTO.DailyMenus.Max(dm => dm.Date);
            weeklyMenuSelectables.Add(weeklyMenuDTO, new WeeklyMenuSelectableVM(menuStartDate, menuEndDate));
        }

        if (weeklyMenus.Count == 0)
        {
            //No menus
            weeklyMenuDTOsLoaded = true;
            StateHasChanged();
            return;
        }

        InitialSelectWeeklyAndDailyMenu();

        weeklyMenuDTOsLoaded = true;
        StateHasChanged();
    }

    /// <summary>
    /// Sets the selected weekly and daily menu for when the user first comes in
    /// based on what i think are good UX rules
    /// </summary>
    private void InitialSelectWeeklyAndDailyMenu()
    {
        //Try to select the best UX weekly menu 

        //If it's saturday or sunday, try to select a menu for the next week:
        //Sa Su Mo Tu We Th Fr
        //#  -  #  #  #  #  #
        //If there is daily menu up to five days from -, select that weekly menu

        //If no matches, just pick the first one

        //Select based on saturday, sunday
        var today = DateTime.Today;
        if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
        {
            var dayOffset = today.DayOfWeek == DayOfWeek.Saturday ? 1 : 0;
            var saturdayDate = today.AddDays(dayOffset);

            //Check for a weekly menu, that has a daily menu with distance to saturday <= 5
            selectedWeeklyMenu = weeklyMenus.FirstOrDefault(wm =>
                wm.DailyMenus.Any(
                    dm => dm.Date > saturdayDate && dm.Date <= saturdayDate.AddDays(5)));
            selectedDailyMenu = selectedWeeklyMenu.DailyMenus.First();
        }

        //Select based on day within this week
        if (selectedWeeklyMenu == default)
        {
            //We didn't select based on it being saturday or sunday
            //Try to pick one from within this week

            var potentialDailyMenu = weeklyMenus
                .SelectMany(w => w.DailyMenus)
                .FirstOrDefault(d => d.Date == today);

            if (potentialDailyMenu is not null)
            {
                selectedDailyMenu = potentialDailyMenu;

                //Dirty hack
                selectedWeeklyMenu = weeklyMenus.First(w =>
                    w.PrimirestMenuId == selectedDailyMenu.Foods[0].PrimirestFoodIdentifier.MenuId);
            }
        }

        //Select fallback first
// ReSharper disable once InvertIf
        if (selectedWeeklyMenu == default)
        {
            selectedWeeklyMenu = weeklyMenus.First();
            selectedDailyMenu = selectedWeeklyMenu.DailyMenus.First();
        }
    }

    //Todo:
    private void OpenDatePicker()
    {

    }

    private void CloseDatePicker()
    {

    }

    private void SelectMenu(WeeklyMenuDTO menu)
    {

    }

    readonly record struct WeeklyMenuSelectableVM(DateTime StartDate, DateTime EndDate);
}