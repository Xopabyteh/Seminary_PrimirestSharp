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
        var foods = await _foodRepository.GetFoodsByPrimirestItemIdAsync(
            ordersResult.Value.Select(o => o.FoodItemId).ToList());

        foreach (var primirestOrder in ordersResult.Value)
        {
            if (!foods.TryGetValue(primirestOrder.FoodItemId, out var food))
            {
                throw new IllegalStateException("There was no match for the ItemId from ordersResult in our repository. This means that the order has a different ItemId than the foods in our system.");
            }

            orders.Add(new Order(food.Id, primirestOrder));
        }

        return orders;
    }
}