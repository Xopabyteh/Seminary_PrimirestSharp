using ErrorOr;
using MediatR;
using Yearly.Application.Menus;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Application.Orders.Queries;

public record GetOrdersForWeekQuery(string SessionCookie, WeeklyMenuId WeekId)
    : IRequest<ErrorOr<IReadOnlyList<Order>>>;