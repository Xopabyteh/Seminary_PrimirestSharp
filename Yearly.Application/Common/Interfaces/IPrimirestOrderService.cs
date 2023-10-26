using ErrorOr;
using MediatR;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestOrderService
{
    public Task<ErrorOr<Unit>> OrderFoodAsync(string sessionCookie, PrimirestOrderIdentifier orderIdentifier);
    public Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestOrderIdentifier orderIdentifier);
}