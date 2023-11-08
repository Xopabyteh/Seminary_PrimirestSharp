using ErrorOr;
using MediatR;
using Yearly.Domain.Models.Common.ValueObjects;

namespace Yearly.Application.Orders.Queries;

public record GetOrdersForWeekQuery(string SessionCookie, int WeeklyMenuId)
    : IRequest<ErrorOr<IReadOnlyList<Order>>>;