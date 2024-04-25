using ErrorOr;
using HtmlAgilityPack;
using Yearly.Application.Orders;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Http;

namespace Yearly.Infrastructure.Services.Orders;

public class PrimirestFinanceService : IPrimirestFinanceService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PrimirestFinanceService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<UserFinanceDetails>> GetFinanceDetailsForUser(string sessionCookie)
    {
        // 1. Load balance page (https://www.mujprimirest.cz/CS/account/balance)
        //   because it's the smallest fetch size
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var response = await client.GetAsync("https://www.mujprimirest.cz/CS/account/balance");

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        // 2. Scrape from sidebar
        // Load document
        var stringContent = await response.Content.ReadAsStringAsync();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(stringContent);

        // Load nodes
        var orderedForXPath = "/html/body/div[1]/div[2]/div[3]/span[2]/a";
        var balanceXPath = "/html/body/div[1]/div[2]/div[3]/a[1]";
        var orderedForNode = htmlDocument.DocumentNode.SelectSingleNode(orderedForXPath);
        var balanceNode = htmlDocument.DocumentNode.SelectSingleNode(balanceXPath);

        if (orderedForNode == null || balanceNode == null)
            throw new InvalidPrimirestContractException("Could not scrape balance from sidebar");

        // Parse finance details
        var orderedForStr = orderedForNode.InnerText;
        var balanceStr = balanceNode.InnerText;

        var didParse = decimal.TryParse(orderedForStr, out var orderedFor);
        if (!didParse)
            throw new InvalidPrimirestContractException("Could not parse ordered for value");

        didParse = decimal.TryParse(balanceStr, out var balance);
        if (!didParse)
            throw new InvalidPrimirestContractException("Could not parse balance value");

        // Return
        return new UserFinanceDetails(new(balance), new(orderedFor));
    }
}