using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Http;
using Yearly.Infrastructure.Services.Menus;
using Yearly.Infrastructure.Services.Orders.ResponseModels;

namespace Yearly.Infrastructure.Services.Orders;

public class PrimirestOrderService : IPrimirestOrderService
{
    private const string k_MensaPurchasePlaceId = "24087276";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PrimirestOrderService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    public PrimirestOrderService(IHttpClientFactory httpClientFactory, ILogger<PrimirestOrderService> logger, IDateTimeProvider dateTimeProvider)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<PrimirestOrderData>> OrderFoodAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var response = await client.GetAsync(
            $"https://www.mujprimirest.cz/ajax/CS/boarding/0/order?menuID={foodIdentifier.MenuId}&dayID={foodIdentifier.DayId}&itemID={foodIdentifier.ItemId}&purchasePlaceID={k_MensaPurchasePlaceId}&_=0"); //Only god knows what the _=xyz is

        if(response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        var responseJson = await response.Content.ReadAsStringAsync();

        //Response is always OK, but there is a "Success" param in body 
        var responseObj = JsonConvert.DeserializeObject<PrimirestOrderResponseRoot>(responseJson)!;

        if (responseObj.Success)
        {
            var item = responseObj.Orders[0].Items[0]; //Why the fuck does primirest store it like this wtf

            var reconstructedOrderIdentifier = new PrimirestFoodOrderIdentifier(
                OrderItemId: item.ID,
                OrderId: item.IDOrder,
                FoodItemId: item.IDItem,
                MenuId: foodIdentifier.MenuId);
            var price = item.BoarderTotalPriceVat;

            var orderData = new PrimirestOrderData(reconstructedOrderIdentifier, new(price));
            return orderData;
        }

        if (responseObj.Message! == @"Časový limit pro objednávky již vypršel")
            return Application.Errors.Errors.Orders.TooLateToOrder;

        if(responseObj.Message! == @"Strávník nemá dostatek peněz na kontě")
            return Application.Errors.Errors.Orders.InsufficientFunds;

        _logger.LogError("Primirest changed error codes");
        return Error.Failure("Unknown");
    }

    public async Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestFoodOrderIdentifier foodIdentifier)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var response = await client.GetAsync(
            $"https://www.mujprimirest.cz/ajax/CS/boarding/0/cancelOrderItem?orderID={foodIdentifier.OrderId}&itemID={foodIdentifier.OrderItemId}&menuID={foodIdentifier.MenuId}&purchasePlaceID={k_MensaPurchasePlaceId}&_=0"); //Only god knows what the _=xyz is

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        var responseJson = await response.Content.ReadAsStringAsync();

        var responseObj = JsonConvert.DeserializeObject<PrimirestOrderResponseRoot>(responseJson);

        if(responseObj.Success)
            return Unit.Value;

        if (responseObj.Message! == @"Časový limit pro zrušení objednávky již vypršel")
            return Application.Errors.Errors.Orders.TooLateToCancelOrder;

        if (responseObj.Message == @"Objednávka nebo některé její položky již byly zkonzumovány")
            return Application.Errors.Errors.Orders.AlreadyConsumed;

        _logger.LogError("Primirest changed error codes");
        return Error.Failure("Unknown");
    }

    public async Task<ErrorOr<IReadOnlyList<PrimirestOrderData>>> GetOrdersForPersonForWeekAsync(string sessionCookie, WeeklyMenuId id)
    {
        // Call the same API as when getting menus
        // This time grab the orders

        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var message = new HttpRequestMessage(
            HttpMethod.Get,
            @$"ajax/CS/boarding/3041/index?purchasePlaceID=24087276&menuID={id.Value}&menuViewType=FULL&_=0");

        var response = await client.SendAsync(message);

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;

        var responseJson = await response.Content.ReadAsStringAsync();

        var responseRoot = JsonConvert.DeserializeObject<PrimirestMenuResponseRoot>(
            responseJson,
            new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }) ?? throw new InvalidPrimirestContractException("Primirest changed their Menu retrieval contract");

        var ordersResponse = responseRoot.Menu.Orders;
        var foodOrders = new List<PrimirestOrderData>(ordersResponse.Count);

        // Reconstruct the orders from the response
        foreach (var orderResponse in ordersResponse)
        {
            var item = orderResponse.Items[0]; //Always only 1 item.
            var reconstructedOrderIdentifier = new PrimirestFoodOrderIdentifier(
                OrderItemId: item.ID,
                OrderId: item.IDOrder,
                FoodItemId: item.IDItem,
                MenuId: id.Value);

            var price = item.BoarderTotalPriceVat;

            var orderData = new PrimirestOrderData(reconstructedOrderIdentifier, new(price));

            foodOrders.Add(orderData);
        }

        return foodOrders;
    }

    public async Task<ErrorOr<MoneyCzechCrowns>> GetBalanceOfUserWithoutOrdersAccountedAsync(string sessionCookie, UserId userId)
    {
        //https://www.mujprimirest.cz/ajax/cs/account/26564871/balance?month=12&year=2023&_=1703936122412
        
        //Not that it really matters, but i am using CzechNow, instead of UTCNow, because Primirest also uses Czech Now
        var monthNumber = _dateTimeProvider.CzechNow.Month;
        var yearNumber = _dateTimeProvider.CzechNow.Year;

        var requestUrl = @$"ajax/cs/account/{userId.Value}/balance?month={monthNumber}&year={yearNumber}&_=0";
        var message = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        message.Headers.Add("Cookie", sessionCookie);

        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);

        var response = await client.SendAsync(message);

        if (response.GotRoutedToLogin())
            return Application.Errors.Errors.Authentication.CookieNotSigned;
        
        var responseJson = await response.Content.ReadAsStringAsync();
        var balanceRoot = JsonConvert.DeserializeObject<BalanceResponseRoot>(responseJson)
            ?? throw new InvalidPrimirestContractException("Primirest changed their Balance retrieval contract");

        var balanceOfUserWithoutOrdersAccounted = balanceRoot.Rows2[0].ClosingBalance;
        return new MoneyCzechCrowns(balanceOfUserWithoutOrdersAccounted);
    }
}