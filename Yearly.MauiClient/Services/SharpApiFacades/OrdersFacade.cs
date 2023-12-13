using System.Net.Http.Json;
using Yearly.Contracts.Common;
using Yearly.Contracts.Order;

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

    public async Task<PrimirestOrderIdentifierDTO> NewOrderAsync(PrimirestFoodIdentifierDTO foodId)
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
            var result = await response.Content.ReadFromJsonAsync<PrimirestOrderIdentifierDTO>();
            return result!;
        }

        //Throw something
        //Todo:
        throw new Exception();
    }

    public async Task CancelOrderAsync(PrimirestOrderIdentifierDTO orderId)
    {
        //Post to {{host}}/order/cancel-order
        var response = await _sharpAPIClient.HttpClient.PostAsJsonAsync(
            "order/cancel-order", 
            new CancelOrderRequest(orderId));

        if (response.IsSuccessStatusCode)
            return;

        //Throw something
        //Todo:
        throw new Exception();

    }
}