using Yearly.Contracts.Order;

namespace Yearly.MauiClient.Services.SharpApiFacades;

public class OrdersFacade
{
    private readonly SharpAPIClient _sharpAPIClient;

    public OrdersFacade(SharpAPIClient sharpAPIClient)
    {
        _sharpAPIClient = sharpAPIClient;
    }

    public async Task<MyOrdersResponse> GetMyOrdersForWeek()
    {
        //var response = await _sharpAPIClient.HttpClient.
        //return response;
        throw new NotImplementedException();
    }
}