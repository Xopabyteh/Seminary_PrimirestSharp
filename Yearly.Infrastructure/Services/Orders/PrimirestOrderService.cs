using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Http;

namespace Yearly.Infrastructure.Services.Orders;

public class PrimirestOrderService : IPrimirestOrderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PrimirestOrderService> _logger;
    public PrimirestOrderService(IHttpClientFactory httpClientFactory, ILogger<PrimirestOrderService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private const string k_MensaPurchasePlaceId = "24087276";
    public async Task<ErrorOr<Unit>> OrderFoodAsync(string sessionCookie, PrimirestOrderIdentifier orderIdentifier)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.Primirest);
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var responseJson = await client.GetStringAsync(
            $"https://www.mujprimirest.cz/ajax/CS/boarding/0/order?menuID={orderIdentifier.MenuId}&dayID={orderIdentifier.DayId}&itemID={orderIdentifier.ItemId}&purchasePlaceID={k_MensaPurchasePlaceId}&_=0"); //Only god knows what the _=xyz is
        
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

    public Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestOrderIdentifier orderIdentifier)
    {
        throw new NotImplementedException();
    }

    private readonly record struct PrimirestOrderResponse(bool Success, string? Message);
}