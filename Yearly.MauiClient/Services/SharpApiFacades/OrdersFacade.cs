using OneOf;
using System.Net.Http.Json;
using Yearly.Contracts.Common;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Exceptions;

namespace Yearly.MauiClient.Services.SharpApiFacades;

public class OrdersFacade
{
    private readonly SharpAPIClient _sharpAPIClient;

    public OrdersFacade(SharpAPIClient sharpAPIClient)
    {
        _sharpAPIClient = sharpAPIClient;
    }

    public async Task<MyOrdersResponse> GetMyOrdersForWeekAsync(int weekId)
    {
        //Fetch for {{host}}/order/my-orders?menuForWeekId=120960615
        var response = await _sharpAPIClient.HttpClient.GetAsync($"order/my-orders?menuForWeekId={weekId}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<MyOrdersResponse>();
        return result!;
    }

    public async Task<OneOf<PrimirestOrderDataDTO, ProblemResponse>> NewOrderAsync(PrimirestFoodIdentifierDTO foodId)
    {
        //Post to {{host}}/order/new-order
        //{
        //    "primirestFoodIdentifier": {
        //        "menuId": 129211623,
        //        "dayId": 129211642,
        //        "itemId": 129211649
        //    }
        //}
        var response = await _sharpAPIClient.HttpClient.PostAsJsonAsync(
            "order/new-order",
            new NewOrderRequest(foodId));
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PrimirestOrderDataDTO>();
            return result!;
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        return problem;
    }

    public async Task<ProblemResponse?> CancelOrderAsync(PrimirestOrderIdentifierDTO orderId)
    {
        //Post to {{host}}/order/cancel-order
        var response = await _sharpAPIClient.HttpClient.PostAsJsonAsync(
            "order/cancel-order", 
            new CancelOrderRequest(orderId));

        if (response.IsSuccessStatusCode)
            return null;

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        return problem;
    }
}