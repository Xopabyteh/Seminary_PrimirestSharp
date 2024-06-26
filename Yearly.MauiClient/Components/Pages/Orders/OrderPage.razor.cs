using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yearly.Contracts.Authentication;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages.Orders;

public partial class OrderPage
{
    [Inject] private MenuAndOrderCacheService _menuAndOrderCacheService { get; set; } = null!;
    [Inject] private IJSRuntime _js { get; set; } = null!;
    [Inject] private DateTimeProvider _dateTimeProvider { get; set; } = null!;
    [Inject] private AuthService _authService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    private IReadOnlyList<WeeklyMenuDTO> weeklyMenus = Array.Empty<WeeklyMenuDTO>();
    private bool weeklyMenuDTOsLoaded = false; //Not the components themselves, but the DTOs.
                                               //Also implies that selected weeklyMenu and dailyMenu are loaded
                                               
    private WeeklyMenuDTO? selectedWeeklyMenu;
    private DailyMenuDTO? selectedDailyMenu;

    private Dictionary<WeeklyMenuDTO, WeeklyMenuSelectableVM> weeklyMenuSelectables = new(3);
    private bool datePickerOpen = false;

    private bool userPickerOpen = false;
    private bool isSwitchingContext = false;

    protected override async Task OnInitializedAsync()
    {
        //Get weekly menus
        await _menuAndOrderCacheService.EnsureMenusLoadedAsync();
        weeklyMenus = _menuAndOrderCacheService.GetAvailableMenus();

        for (int i = 0; i < weeklyMenus.Count; i++)
        {
            WeeklyMenuDTO weeklyMenuDTO = weeklyMenus[i];
            var menuStartDate = weeklyMenuDTO.DailyMenus.Min(dm => dm.Date);
            var menuEndDate = weeklyMenuDTO.DailyMenus.Max(dm => dm.Date);
            weeklyMenuSelectables.Add(weeklyMenuDTO, new WeeklyMenuSelectableVM(menuStartDate, menuEndDate));
        }

        if (weeklyMenus.Count == 0)
        {
            // -> No menus
            weeklyMenuDTOsLoaded = true;
            return;
        }
        
        // -> Yes menus
        weeklyMenuDTOsLoaded = true;
        InitialSelectWeeklyAndDailyMenu();
    }

    /// <summary>
    /// Sets the selected weekly and daily menu for when the user first comes in
    /// based on what I think are good UX rules.
    /// </summary>
    private void InitialSelectWeeklyAndDailyMenu()
    {
        if (weeklyMenus.Count == 0)
            throw new WeeklyMenusAreEmptyException(); //Don't call this method if there are no weekly menus

        //Try to select the best UX weekly menu 

        //If it's saturday or sunday, try to select a menu for the next week:
        //Sa Su Mo Tu We Th Fr
        //#  -  #  #  #  #  #
        //If there is daily menu up to five days from -, select that weekly menu

        //If no matches, just pick the first one

        //Select based on saturday, sunday
        var today = _dateTimeProvider.Today;
        if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
        {
            var dayOffset = today.DayOfWeek == DayOfWeek.Saturday ? 1 : 0;
            var saturdayDate = today.AddDays(dayOffset);

            //Check for a weekly menu, that has a daily menu with distance to saturday <= 5
            selectedWeeklyMenu = weeklyMenus.FirstOrDefault(wm =>
                wm.DailyMenus.Any(
                    dm => dm.Date > saturdayDate && dm.Date <= saturdayDate.AddDays(5)));
        }

        //Select based on day within this week
        if (selectedWeeklyMenu is null)
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
        selectedWeeklyMenu ??= weeklyMenus[0];
        selectedDailyMenu ??= selectedWeeklyMenu?.DailyMenus.FirstOrDefault(); //I hope it never comes to the day, where there is a weekly menu with 0 daily menus,
                                                                                    //but oh lord, it's primirest, so anything can happen
    }

    private void OpenDatePicker()
    {
        if (weeklyMenuSelectables.Count <= 1)
            return; // No other options to pick from

        datePickerOpen = true;
    }

    private void CloseDatePicker()
    {
        datePickerOpen = false;
    }

    private void OpenUserPicker()
    {
        if(_authService.AvailableUsersWithinTenant!.Count <= 1)
            return; // No other options to pick from

        userPickerOpen = true;
    }

    private void CloseUserPicker()
    {
        userPickerOpen = false;
    }

    /// <summary>
    /// Called through date picker
    /// Sets selected menu and scrolls to top
    /// </summary>
    /// <param name="menu"></param>
    /// <returns></returns>
    private async ValueTask SelectMenu(WeeklyMenuDTO menu)
    {
        if (!datePickerOpen)
            return;

        await _menuAndOrderCacheService.EnsureMenusLoadedAsync();

        if (selectedWeeklyMenu == menu)
        {
            CloseDatePicker();
            return;
        }

        // Reselect menu and scroll to top
        selectedWeeklyMenu = menu;
        await _js.InvokeVoidAsync("window.scrollTo", 0, 0);
        CloseDatePicker();
    }

    readonly record struct WeeklyMenuSelectableVM(DateTime StartDate, DateTime EndDate);

    class WeeklyMenusAreEmptyException : Exception;

    private async Task SwitchContextAsync(UserDetailsResponse newUser)
    {
        if (newUser == _authService.ActiveUserDetailsLazy)
        {
            CloseUserPicker();
            return; // Same user as is active rn
        }

        isSwitchingContext = true;
        StateHasChanged();

        await _authService.SwitchContextAsync(newUser);
        _navigationManager.Refresh(true);
    }
}