using ErrorOr;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Application.Menus;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuProvider : IPrimirestMenuProvider
{
    private readonly PrimirestAuthService _authService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<PrimirestMenuProvider> _logger;
    private readonly IFoodRepository _foodRepository;

    public PrimirestMenuProvider(PrimirestAuthService authService, IDateTimeProvider dateTimeProvider, IWeeklyMenuRepository weeklyMenuRepository, ILogger<PrimirestMenuProvider> logger, IFoodRepository foodRepository)
    {
        _authService = authService;
        _dateTimeProvider = dateTimeProvider;
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
        _foodRepository = foodRepository;
    }

    public async Task<ErrorOr<List<Food>>> PersistAvailableMenusAsync()
    {
        await DeleteOldMenusAsync();

        //Load menu from primirest
        var primirestMenusResult = await GetMenusThisWeekAsync();
        if (primirestMenusResult.IsError)
            return primirestMenusResult.Errors;

        //Check if there are any
        var primirestWeeklyMenus = primirestMenusResult.Value;
        if (primirestWeeklyMenus.Count == 0)
            return new List<Food>(); //No menus available -> nothing to persist

        var newlyPersistedFoods = await PersistNewMenusAsync(primirestWeeklyMenus);

        return newlyPersistedFoods;
    }

    /// <summary>
    /// Scrape the index page of Primirest to get the menu ids
    /// </summary>
    /// <returns></returns>
    private static async Task<string[]> GetMenuIds(HttpClient loggedClient)
    {
        //Get index page
        var indexPageHtml = await loggedClient.GetStringAsync(
            "CS/boarding/index?purchasePlaceID=24087276&suppressOptionsDialog=true&menuViewType=SIMPLE");

        //Scrape the menu ids
        var indexPageDoc = new HtmlDocument();
        indexPageDoc.LoadHtml(indexPageHtml);

        const string xpath = @"//div[@class='menu-select panel-control responsive-control']//select//option";
        var idOptionElements = indexPageDoc.DocumentNode.SelectNodes(xpath);
        var idsResult = idOptionElements
            .Select(e => e.GetAttributeValue("value", string.Empty))
            .ToArray();

        return idsResult;
    }

    /// <summary>
    /// Fetch menus from primirest
    /// Nothing else is done, only fetching
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<ErrorOr<List<PrimirestWeeklyMenu>>> GetMenusThisWeekAsync()
    {
        //In order to get the menus from Primirest, we need to call their menu API.
        //But since it sucks, we have to request by some arcane wizard random id
        //The ids can only be fetched by scraping their index page

        //Also, the food names include the soup name in them
        //It is always the same and is the first part of the food name string
        //To get the soup, we have to find the common part of the food names
        //And then remove it from the food names

        static string GetSoupFromRawFoodNames(List<string> foodNames)
        {
            static string GetCommonSubstring(string str1, string str2)
            {
                string commonSubstring = string.Empty;
                int len1 = str1.Length;
                int len2 = str2.Length;
                for (int i = 0; i < len1; i++)
                {
                    for (int j = i + 1; j <= len1; j++)
                    {
                        string subString = str1[i..j];
                        if (len2 >= subString.Length && str2.Contains(subString))
                        {
                            if (subString.Length > commonSubstring.Length)
                            {
                                commonSubstring = subString;
                            }
                        }
                    }
                }
                return commonSubstring;
            }

            string commonSubstring = foodNames.Aggregate(GetCommonSubstring);
            return commonSubstring;
        }

        return await _authService.PerformAdminLoggedSessionAsync<List<PrimirestWeeklyMenu>>(async loggedClient =>
        {
            //Fetch menu ids from primirest
            var menuIds = await GetMenuIds(loggedClient);

            var weeklyMenus = new List<PrimirestWeeklyMenu>(menuIds.Length);
            foreach (var menuId in menuIds)
            {
                var message = new HttpRequestMessage(
                    HttpMethod.Get,
                    @$"ajax/CS/boarding/3041/index?purchasePlaceID=24087276&menuID={menuId}&menuViewType=FULL&_=0");

                var response = await loggedClient.SendAsync(message);
                var responseJson = await response.Content.ReadAsStringAsync();

                var responseRoot = JsonConvert.DeserializeObject<PrimirestMenuResponseRoot>(
                    responseJson,
                    new JsonSerializerSettings()
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    }) ?? throw new InvalidPrimirestContractException("Primirest changed their Menu retrieval contract");

                var weeklyMenu = new PrimirestWeeklyMenu(new(5), int.Parse(menuId));

                foreach (var day in responseRoot.Menu.Days)
                {
                    //Construct menu per day here

                    var foods = new List<PrimirestFood>(3); 
                    var menuDate = day.Date.AddHours(1); //Primirest stores in cz, we parse in utc, so we add 1 to go back to cz

                    var rawFoodNames = new List<string>(3); //Foods with soup name in them
                    foreach (var item in day.Items)
                    {
                        var rawFoodName = item.Description;
                        rawFoodNames.Add(rawFoodName);
                    }

                    //Find the common part of the food names
                    var soupName = GetSoupFromRawFoodNames(rawFoodNames);

                    //Construct food objects
                    foreach (var item in day.Items)
                    {
                        var foodName = item.Description.Replace(soupName, string.Empty).Trim();

                        var primirestIdentifier = new PrimirestFoodIdentifier(
                            item.IDMenu,
                            item.IDMenuDay,
                            item.ID);

                        var food = new PrimirestFood(foodName, item.MealAllergensMarkings, primirestIdentifier);
                        foods.Add(food);
                    }

                    var soup = new PrimirestSoup(soupName.Trim(',', ' '));
                    
                    //Construct menu for this day
                    var dailyMenu = new PrimirestDailyMenu(menuDate, foods, soup);
                    //reconstructedMenus.Add(menu);

                    weeklyMenu.DailyMenus.Add(dailyMenu);
                }

                weeklyMenus.Add(weeklyMenu);
            }

            return weeklyMenus;
        });
    }

    private async Task DeleteOldMenusAsync()
    {
        var today = _dateTimeProvider.CzechNow;
        var deletedCount = await _weeklyMenuRepository.ExecuteDeleteMenusBeforeDateAsync(today);
        _logger.LogInformation("Deleted {count} old menus before the date {date}", deletedCount, today);
    }

    /// <returns>Returns newly persisted foods</returns>
    private async Task<List<Food>> PersistNewMenusAsync(List<PrimirestWeeklyMenu> primirestWeeklyMenusToPersist)
    {
        var newlyPersistedFoods = new List<Food>(primirestWeeklyMenusToPersist.Count * 3);
        foreach (var primirestWeeklyMenu in primirestWeeklyMenusToPersist)
        {
            var weeklyMenuId = new WeeklyMenuId(primirestWeeklyMenu.PrimirestMenuId);

            //If we already have this menu, skip it
            //We only want to persist menus and foods from menus, that we don't have yet
            if (await _weeklyMenuRepository.DoesMenuExist(weeklyMenuId))
                continue;

            var dailyMenus = new List<DailyMenu>(5);

            //Handle foods from this weekly menu
            foreach (var primirestDailyMenu in primirestWeeklyMenu.DailyMenus)
            {
                var foodIdsForDay = new List<FoodId>(3);

                //Handle foods
                foreach (var primirestFood in primirestDailyMenu.Foods)
                {
                    var food = Food.Create(
                        primirestFood.Name,
                        primirestFood.Allergens,
                        primirestFood.PrimirestFoodIdentifier);

                    _logger.Log(LogLevel.Information, "New food created - {foodName}", food.Name);
                    newlyPersistedFoods.Add(food);
                    await _foodRepository.AddFoodAsync(food); //Todo: optimize with addRange

                    foodIdsForDay.Add(food.Id);
                }

                var menuForDay = new DailyMenu(foodIdsForDay, primirestDailyMenu.Date);
                dailyMenus.Add(menuForDay);
            }

            //Construct menu for week and store it
            var menuForWeek = WeeklyMenu.Create(weeklyMenuId, dailyMenus);
            await _weeklyMenuRepository.AddMenuAsync(menuForWeek);
        }

        return newlyPersistedFoods;
    }
}