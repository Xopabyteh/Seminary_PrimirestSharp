using ErrorOr;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuProvider : IPrimirestMenuProvider
{
    private readonly IPrimirestAdminLoggedSessionRunner _loggedSessionRunner;

    public PrimirestMenuProvider(IPrimirestAdminLoggedSessionRunner loggedSessionRunner)
    {
        _loggedSessionRunner = loggedSessionRunner;
    }

    public async Task<ErrorOr<List<PrimirestWeeklyMenu>>> GetMenusThisWeekAsync()
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

        return await _loggedSessionRunner.PerformAdminLoggedSessionAsync<List<PrimirestWeeklyMenu>>(async loggedClient =>
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
                    var menuDate = day.Date.AddHours(2); //Primirest stores in cz, we parse in utc, so we add 2 to go back to cz

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
    /// Scrape the index page of Primirest to get the menu ids
    /// </summary>
    /// <returns></returns>
    public async Task<int[]> GetMenuIdsAsync(HttpClient adminSessionLoggedClient)
    {
        //Get index page
        var indexPageHtml = await adminSessionLoggedClient.GetStringAsync(
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