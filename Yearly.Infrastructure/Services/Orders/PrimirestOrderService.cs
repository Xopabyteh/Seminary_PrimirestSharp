using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Application.Menus;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Http;

namespace Yearly.Infrastructure.Services.Orders;

public class PrimirestOrderService : IPrimirestOrderService
{
    private const string k_MensaPurchasePlaceId = "24087276";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PrimirestOrderService> _logger;
    public PrimirestOrderService(IHttpClientFactory httpClientFactory, ILogger<PrimirestOrderService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ErrorOr<Unit>> OrderFoodAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var responseJson = await client.GetStringAsync(
            $"https://www.mujprimirest.cz/ajax/CS/boarding/0/order?menuID={foodIdentifier.MenuId}&dayID={foodIdentifier.DayId}&itemID={foodIdentifier.ItemId}&purchasePlaceID={k_MensaPurchasePlaceId}&_=0"); //Only god knows what the _=xyz is
        
        //Response is always OK, but there is a "Success" param in body 
        var response = JsonConvert.DeserializeObject<PrimirestOrderResponse>(responseJson);
        
        if(response.Success)
            return Unit.Value;

        if (response.Message! == @"Časový limit pro objednávky již vypršel")
            return Application.Errors.Errors.Orders.TooLateToOrder;

        if(response.Message! == @"Strávník nemá dostatek peněz na kontě")
            return Application.Errors.Errors.Orders.InsufficientFunds;

        _logger.LogError("Primirest changed error codes");
        return Error.Failure("Unknown");
    }

    public async Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestFoodOrderIdentifier foodIdentifier)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var responseJson = await client.GetStringAsync(
            $"https://www.mujprimirest.cz/ajax/CS/boarding/0/cancelOrderItem?orderID={foodIdentifier.OrderId}&itemID={foodIdentifier.OrderItemId}&menuID={foodIdentifier.MenuId}&purchasePlaceID={k_MensaPurchasePlaceId}&_=0"); //Only god knows what the _=xyz is

        var response = JsonConvert.DeserializeObject<PrimirestOrderResponse>(responseJson);

        if(response.Success)
            return Unit.Value;

        if (response.Message! == @"Časový limit pro zrušení objednávky již vypršel")
            return Application.Errors.Errors.Orders.TooLateToCancelOrder;

        _logger.LogError("Primirest changed error codes");
        return Error.Failure("Unknown");
    }

    public async Task<ErrorOr<IReadOnlyList<PrimirestFoodOrderIdentifier>>> GetOrdersForPersonForWeekAsync(string sessionCookie, WeeklyMenuId id)
    {
        // Call the same API as when getting menus
        // This time grab the orders

        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var message = new HttpRequestMessage(
            HttpMethod.Get,
            @$"ajax/CS/boarding/3041/index?purchasePlaceID=24087276&menuID={id.Value}&menuViewType=FULL&_=0");

        //Todo: handle session cookie not signed error here

        var response = await client.SendAsync(message);
        var responseJson = await response.Content.ReadAsStringAsync();

        var responseRoot = JsonConvert.DeserializeObject<PrimirestMenuResponseRoot>(
            responseJson,
            new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }) ?? throw new InvalidPrimirestContractException("Primirest changed their Menu retrieval contract");

        var ordersResponse = responseRoot.Menu.Orders;
        var foodOrders = new List<PrimirestFoodOrderIdentifier>(ordersResponse.Count);

        // Reconstruct the orders from the response
        foreach (var orderResponse in ordersResponse)
        {
            var item = orderResponse.Items[0]; //Always only 1 item.
            var reconstructedOrder = new PrimirestFoodOrderIdentifier(
                OrderItemId: item.ID,
                OrderId: item.IDOrder,
                FoodItemId: item.IDItem,
                MenuId: id.Value);

            foodOrders.Add(reconstructedOrder);
        }

        return foodOrders;
    }

    private readonly record struct PrimirestOrderResponse(bool Success, string? Message);
}