using ErrorOr;
using HtmlAgilityPack;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services.Authentication;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuProviderService : IMenuProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthService _authService;

    public PrimirestMenuProviderService(IHttpClientFactory httpClientFactory, IAuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    public async Task<ErrorOr<List<Menu>>> GetMenusThisWeek()
    {
        //In order to get the menus from Primirest, we need to call their menu API.
        //But since it sucks, we have to request by some arcane wizard random id
        //The ids can only be fetched by scraping their index page
        var menuIds = await GetMenuIds();


        return Array.Empty<Menu>().ToList();
    }

    /// <summary>
    /// Scrape the index page of Primirest to get the menu ids
    /// </summary>
    /// <returns></returns>
    private async Task<ErrorOr<string[]>> GetMenuIds()
    {
        //TODO: Move these hardcoded credentials
        //Login as admin
        var username = @"ICOM7620.00071326564871";
        var password = @"Martin.321";

        var loginResult = await _authService.LoginAsync(username, password);
        
        if (loginResult.IsError)
            return Infrastructure.Errors.Errors.System.InvalidAdminCredentials;
        
        var sessionCookie = loginResult.Value.SessionCookie;
        
        //Get index page
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);

        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);
        var indexPageHtml = await client.GetStringAsync(
        "CS/boarding/index?purchasePlaceID=24087276&suppressOptionsDialog=true&menuViewType=SIMPLE");

        //Logout
        client.DefaultRequestHeaders.Remove("Cookie");
        //await _authService.LogoutAsync(sessionCookie);


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