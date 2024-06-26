﻿using ErrorOr;
using MediatR;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Orders;

public interface IPrimirestOrderService
{
    public Task<ErrorOr<PrimirestOrderData>> OrderFoodAsync(string sessionCookie, PrimirestFoodIdentifier foodIdentifier);
    public Task<ErrorOr<Unit>> CancelOrderAsync(string sessionCookie, PrimirestFoodOrderIdentifier foodIdentifier);

    /// <summary>
    /// Get the foods that a person has ordered for the given week
    /// </summary>
    /// <param name="sessionCookie"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<ErrorOr<IReadOnlyList<PrimirestOrderData>>> GetOrdersForPersonForWeekAsync(string sessionCookie, WeeklyMenuId id);
}