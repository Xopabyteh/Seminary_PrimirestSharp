using System.Collections.Concurrent;
using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Orders;

/// <summary>
/// Simulate some sort of ordering with in memory orders cache.
/// Cache is based per session, not per user. Cache isn't invalidated.
///
/// This service is as evil as it gets... (lol)
/// </summary>
public class PrimirestOrderServiceDev : IPrimirestOrderService
{
    private readonly ConcurrentDictionary<string, List<PrimirestOrderData>> _sessionOrders = new();

    public Task<ErrorOr<PrimirestOrderData>> OrderFoodAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier)
    {
        // Hopefully this evil magic is random enough (lol)
        var newOrder = new PrimirestOrderData(
            new PrimirestFoodOrderIdentifier(
                new object().GetHashCode(),
                new object().GetHashCode(),
                foodIdentifier.MenuId,
                foodIdentifier.ItemId),
            new(55));

        if (_sessionOrders.TryGetValue(sessionCookie, out var orders))
        {
            orders.Add(newOrder);
        }
        else
        {
            _sessionOrders[sessionCookie] = new List<PrimirestOrderData> { newOrder };
        }

        return Task.FromResult((ErrorOr<PrimirestOrderData>)newOrder);
    }

    public Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestFoodOrderIdentifier foodIdentifier)
    {
        if (!_sessionOrders.TryGetValue(sessionCookie, out var orders))
            return Task.FromResult((ErrorOr<Unit>)Unit.Value);

        var order = orders.FirstOrDefault(o => o.PrimirestFoodOrderIdentifier == foodIdentifier);
        if (order is not null)
        {
            orders.Remove(order);
        }

        return Task.FromResult((ErrorOr<Unit>)Unit.Value);
    }

    public Task<ErrorOr<IReadOnlyList<PrimirestOrderData>>> GetOrdersForPersonForWeekAsync(string sessionCookie, WeeklyMenuId id)
    {
        var hasOrdersSetup = _sessionOrders.TryGetValue(sessionCookie, out var orders);
        if (!hasOrdersSetup)
        {
            _sessionOrders[sessionCookie] = new List<PrimirestOrderData>();
            return Task.FromResult((ErrorOr<IReadOnlyList<PrimirestOrderData>>)new List<PrimirestOrderData>());
        }
        return Task.FromResult((ErrorOr<IReadOnlyList<PrimirestOrderData>>)orders!);
    }

    public Task<ErrorOr<MoneyCzechCrowns>> GetBalanceOfUserWithoutOrdersAccountedAsync(string sessionCookie, UserId userId)
    {
        return Task.FromResult((ErrorOr<MoneyCzechCrowns>)new MoneyCzechCrowns(1337));
    }
}