using System.Globalization;
using System.Net.Http.Json;
using ErrorOr;
using Yearly.Application.Orders;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services.Orders.ResponseModels;

namespace Yearly.Infrastructure.Services.Orders;

public class PrimirestFinanceService : IPrimirestFinanceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PrimirestFinanceService(IHttpClientFactory httpClientFactory, IDateTimeProvider dateTimeProvider)
    {
        _httpClientFactory = httpClientFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<UserFinanceDetails>> GetFinanceDetailsForUser(string sessionCookie, User ofUser)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var getOrderedForTask = GetUserOrderedFor(client, ofUser);
        var getBalanceTask = GetUserBalance(client, ofUser);

        await Task.WhenAll(getOrderedForTask, getBalanceTask);

        var orderedFor = getOrderedForTask.Result;
        var balance = getBalanceTask.Result;

        if(orderedFor.IsError || balance.IsError)
            return orderedFor.ErrorsOrEmptyList.Concat(balance.ErrorsOrEmptyList).ToArray();

        return new UserFinanceDetails(balance.Value, orderedFor.Value);
    }

    private async Task<ErrorOr<MoneyCzechCrowns>> GetUserOrderedFor(
        HttpClient loggedClient,
        User ofUser)
    {
        // Load https://www.mujprimirest.cz/ajax/cs/ordersummary/{UserId}/index?id={UserId}&from=1.5.2024&to=31.05.2024&stateCodes=INACTIVE,ACTIVE,COMPLETED&dispenseState=-1&_=0

        // Today in format "dd.mm.yyyy"
        var from = new DateTime(
                _dateTimeProvider.CzechNow.Year,
                _dateTimeProvider.CzechNow.Month, 
                _dateTimeProvider.CzechNow.Day)
            .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

        // Last day of next month in format "dd.mm.yyyy"
        // We pick the next month, because we predict, that the user will have orders
        // at most until the end of the next month
        // Note: If the balance displayed is incorrect, it is probably due to this being just a prediction
        // and a proper calculation may be required (but damn who expects primirest to push out more than a month of menus)
        var nextMonth = _dateTimeProvider.CzechNow.AddMonths(1);
        var to = new DateTime(
            nextMonth.Year,
            nextMonth.Month,
            DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month))
            .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

        var request = $"https://www.mujprimirest.cz/ajax/cs/ordersummary/{ofUser.Id.Value}/index" +
                      $"?id={ofUser.Id.Value}" +
                      $"&from={from}" +
                      $"&to={to}" +
                      $"&stateCodes=INACTIVE,ACTIVE,COMPLETED" +
                      $"&dispenseState=-1" +
                      $"&_={((DateTimeOffset)_dateTimeProvider.UtcNow).ToUnixTimeSeconds()}";

        var response = await loggedClient.GetAsync(request);

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        var result = await response.Content.ReadFromJsonAsync<PrimirestOrderSummaryResponseRoot>();
        if(result is null)
            throw new InvalidPrimirestContractException("PrimirestOrderSummaryResponseRoot is null");

        var orderedPrice = result.Rows.Sum(x => x.UnitPrice);
        return new MoneyCzechCrowns(orderedPrice);
    }

    private async Task<ErrorOr<MoneyCzechCrowns>> GetUserBalance(
        HttpClient loggedClient,
        User ofUser)
    {
        // https://www.mujprimirest.cz/ajax/cs/account/{AccountId}/balance?month=5&year=2024&_=1714563024737

        var request = $"https://www.mujprimirest.cz/ajax/cs/account/{ofUser.Id.Value}/balance" +
                      $"?month={_dateTimeProvider.CzechNow.Month}" +
                      $"&year={_dateTimeProvider.CzechNow.Year}" +
                      $"&_={((DateTimeOffset)_dateTimeProvider.UtcNow).ToUnixTimeSeconds()}";
        
        var response = await loggedClient.GetAsync(request);

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        var result = await response.Content.ReadFromJsonAsync<BalanceResponseRoot>();
        if(result is null)
            throw new InvalidPrimirestContractException("BalanceResponseRoot is null");

        var balanceRow = result.Rows2.SingleOrDefault();
        if(balanceRow is null)
            throw new InvalidPrimirestContractException("BalanceResponseRoot.Rows2 is null (There should always be one)");
        
        var balance = balanceRow.ClosingBalance;
        return new MoneyCzechCrowns(balance);
    }
}