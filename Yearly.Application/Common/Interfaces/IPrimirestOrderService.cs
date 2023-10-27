using ErrorOr;
using MediatR;
using Yearly.Application.Menus;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestOrderService
{
    public Task<ErrorOr<Unit>> OrderFoodAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier);
    public Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier);
    public Task<ErrorOr<List<PrimirestFoodOrder>>> GetOrdersForPersonAsync(string sessionCookie);
}