using ErrorOr;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Application.Menus;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuProvider : IPrimirestMenuProvider
{
    private readonly PrimirestAuthService _authService;

    public PrimirestMenuProvider(PrimirestAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Fetch menus from primirest
    /// Nothing else is done, only fetching
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ErrorOr<List<PrimirestMenuForWeek>>> GetMenusThisWeekAsync()
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

        return await _authService.PerformAdminLoggedSessionAsync<List<PrimirestMenuForWeek>>(async loggedClient =>
        {
            //Fetch menu ids from primirest
            var menuIds = await GetMenuIds(loggedClient);

            var menusForWeeks = new List<PrimirestMenuForWeek>(menuIds.Length);
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
                    });

                if (responseRoot is null)
                    return Errors.Errors.PrimirestAdapter.PrimirestResponseIsNull;

                var menuForWeek = new PrimirestMenuForWeek(new(5), int.Parse(menuId));

                foreach (var day in responseRoot.Menu.Days)
                {
                    //Construct menu per day here

                    var foods = new List<PrimirestFood>(3); 
                    var menuDate = day.Date.AddDays(1); //Primirest stores in cz, we parse in utc, so we add 1 to go back to cz

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
                    var menu = new PrimirestMenuForDay(menuDate, foods, soup);
                    //reconstructedMenus.Add(menu);

                    menuForWeek.MenusForDay.Add(menu);
                }

                menusForWeeks.Add(menuForWeek);
            }

            return menusForWeeks;
        });
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
}