﻿using ErrorOr;
using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Persistence.Repositories;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuProvider : IPrimirestMenuProvider
{
    private readonly PrimirestAuthService _authService;
    private readonly WeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<PrimirestMenuProvider> _logger;
    private readonly FoodRepository _foodRepository;

    public PrimirestMenuProvider(
        PrimirestAuthService authService, 
        WeeklyMenuRepository weeklyMenuRepository,
        ILogger<PrimirestMenuProvider> logger, 
        FoodRepository foodRepository)
    {
        _authService = authService;
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
        _foodRepository = foodRepository;
    }

    public async Task<ErrorOr<List<Food>>> PersistAvailableMenusAsync()
    {
        // Load menus from primirest
        var primirestMenusResult = await GetMenusThisWeekAsync();
        if (primirestMenusResult.IsError)
            return primirestMenusResult.Errors;

        // Check if there are any menus
        var primirestWeeklyMenus = primirestMenusResult.Value;
        if (primirestWeeklyMenus.Count == 0)
            return new List<Food>(); //No menus available -> nothing to persist

        // If there are new menus, persist them
        var newlyPersistedFoods = await PersistNewMenusAsync(primirestWeeklyMenus);

        return newlyPersistedFoods;
    }

    /// <summary>
    /// Delete menus from our db, that are no longer accessible through primirest
    /// </summary>
    /// <returns></returns>
    public Task<ErrorOr<Unit>> DeleteOldMenusAsync()
    {
        return _authService.PerformAdminLoggedSessionAsync<Unit>(async loggedClient =>
        {
            var menusOnPrimirest = await GetMenuIdsAsync(loggedClient);
            var menusInDb = await _weeklyMenuRepository.GetWeeklyMenuIdsAsync();

            var menusToDelete = menusInDb
                .Where(id => !menusOnPrimirest.Contains(id.Value))
                .ToList();

            await _weeklyMenuRepository.ExecuteDeleteMenusAsync(menusToDelete);

            return Unit.Value;
        });
    }

    /// <summary>
    /// Fetch menus from primirest
    /// Nothing else is done, only fetching
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    //Internal for testing purposes
    internal async Task<ErrorOr<List<PrimirestWeeklyMenu>>> GetMenusThisWeekAsync()
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
            var menuIds = await GetMenuIdsAsync(loggedClient);

            var weeklyMenus = new List<PrimirestWeeklyMenu>(menuIds.Length);
            foreach (var menuId in menuIds)
            {
                var message = new HttpRequestMessage(
                    HttpMethod.Get,
                    @$"ajax/CS/boarding/3041/index?purchasePlaceID=24087276&menuID={menuId}&menuViewType=FULL&_=0");

                var response = await loggedClient.SendAsync(message);

                if (response.GotRoutedToLogin())
                    throw new InvalidAdminCredentialsException("Admin credentials are invalid (probably)");

                var responseJson = await response.Content.ReadAsStringAsync();

                var responseRoot = JsonConvert.DeserializeObject<PrimirestMenuResponseRoot>(
                    responseJson,
                    new JsonSerializerSettings()
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    }) ?? throw new InvalidPrimirestContractException("Primirest changed their Menu retrieval contract");

                var weeklyMenu = new PrimirestWeeklyMenu(new(5), menuId);

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

    /// <summary>
    /// Persists foods from primirest to sharp repositories
    /// and returns the newly persisted foods (which are essentially remapped primirest foods)
    /// </summary>
    /// <returns>Returns newly persisted foods</returns>
    //Internal for testing purposes
    internal async Task<List<Food>> PersistNewMenusAsync(List<PrimirestWeeklyMenu> primirestWeeklyMenusToPersist)
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
                    //Don't add foods that have the same primirest food identifier
                    //It might happen that the delete old menus function miss behaves
                    // and we will get duplicate primirest foods in our db. That is bad
                    if (await _foodRepository.DoesFoodWithPrimirestIdentifierExistAsync(primirestFood
                            .PrimirestFoodIdentifier))
                    {
                        _logger.LogWarning("Food with primirest identifier {identifier} already exists in our db. Skipping it.", primirestFood.PrimirestFoodIdentifier);
                        continue;
                    }

                    var food = Food.Create(
                        new FoodId(Guid.NewGuid()),
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

    /// <summary>
    /// Scrape the index page of Primirest to get the menu ids
    /// </summary>
    /// <returns></returns>
    private static async Task<int[]> GetMenuIdsAsync(HttpClient loggedClient)
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
            .Select(int.Parse)
            .ToArray();

        return idsResult;
    }
}