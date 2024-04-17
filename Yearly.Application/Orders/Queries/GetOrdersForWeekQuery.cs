using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Orders.Queries;

public record GetOrdersForWeekQuery(string SessionCookie, WeeklyMenuId WeekId)
    : IRequest<ErrorOr<IReadOnlyList<Order>>>;

public class GetOrdersForWeekQueryHandler : IRequestHandler<GetOrdersForWeekQuery, ErrorOr<IReadOnlyList<Order>>>
{
    private readonly IPrimirestOrderService _primirestOrderService;
    private readonly IFoodRepository _foodRepository;

    public GetOrdersForWeekQueryHandler(IPrimirestOrderService primirestOrderService, IFoodRepository foodRepository)
    {
        _primirestOrderService = primirestOrderService;
        _foodRepository = foodRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<Order>>> Handle(GetOrdersForWeekQuery request, CancellationToken cancellationToken)
    {
        var ordersResult = await _primirestOrderService.GetOrdersForPersonForWeekAsync(request.SessionCookie, request.WeekId);
        if (ordersResult.IsError)
            return ordersResult.Errors;

        var orders = new List<Order>(ordersResult.Value.Count);

        // Optimization, so we don't have to go to the repository a million times
        var foods = await _foodRepository.GetFoodsByPrimirestItemIdsAsync(
            ordersResult.Value.Select(od => od.PrimirestFoodOrderIdentifier.FoodItemId).ToList());

        foreach (var primirestOrderData in ordersResult.Value)
        {
            if (!foods.TryGetValue(primirestOrderData.PrimirestFoodOrderIdentifier.FoodItemId, out var food))
            {
                throw new IllegalStateException("There was no match for the ItemId from ordersResult in our repository. This means that the order has a different ItemId than any of the foods in sharp.");
            }

            orders.Add(new Order(food.Id, primirestOrderData));
        }

        return orders;
    }
}