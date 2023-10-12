using ErrorOr;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuProviderService : IMenuProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PrimirestAuthService _authService;

    public PrimirestMenuProviderService(IHttpClientFactory httpClientFactory, PrimirestAuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    //TODO: Do this.
    public async Task<ErrorOr<List<Menu>>> GetMenusThisWeek()
    {
        //In order to get the menus from Primirest, we need to call their menu API.
        //But since it sucks, we have to request by some arcane wizard random id
        //The ids can only be fetched by scraping their index page
        return await _authService.PerformAdminLoggedSessionAsync(async loggedClient =>
        {
            //Fetch menus from primirest
            var menuIds = await GetMenuIds(loggedClient);
            foreach (var menuId in menuIds)
            {
                var message = new HttpRequestMessage(HttpMethod.Get,
                    @$"ajax/CS/boarding/3041/index?purchasePlaceID=24087276&menuID={menuId}&menuViewType=FULL&_=0");

                var response = await loggedClient.SendAsync(message);
                var responseJson = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject(responseJson) ?? throw new InvalidOperationException();

                dynamic days = responseObject.Menu.Days;

                foreach (dynamic day in days)
                {
                    DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds(day.Date);
                    foreach (dynamic item in day.Items)
                    {
                        string foodName = item.Meals[0].Meal.Name;

                        //Check if we have a food with this name
                        //Todo:
                    }
                }

            }

            //Check if we have foods inside of the menu
            //If not, create them and persist them

            //Create menu objects

            return Array.Empty<Menu>().ToList();
        });
    }

    /// <summary>
    /// Scrape the index page of Primirest to get the menu ids
    /// </summary>
    /// <returns></returns>
    private async Task<string[]> GetMenuIds(HttpClient loggedClient)
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

    //private record struct PrimirestMenuResponse(
    //    List<Day> Days
    //);

    //private record struct Day(
    //    DateTime Date,
    //    List<Item> Items);

    //private record struct Item(
    //    string Description
    //);
}