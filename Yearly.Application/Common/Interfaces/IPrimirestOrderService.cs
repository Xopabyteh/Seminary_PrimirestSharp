using ErrorOr;
using MediatR;
using Yearly.Application.Menus;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestOrderService
{
    public Task<ErrorOr<PrimirestFoodOrderIdentifier>> OrderFoodAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier);
    public Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestFoodOrderIdentifier foodIdentifier);

    /// <summary>
    /// Get the foods that a person has ordered for the given week
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ErrorOr<IReadOnlyList<PrimirestFoodOrderIdentifier>>> GetOrdersForPersonForWeekAsync(string sessionCookie, WeeklyMenuId id);
}