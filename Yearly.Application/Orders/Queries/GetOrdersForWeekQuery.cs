using ErrorOr;
using MediatR;
using Yearly.Application.Menus;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Application.Orders.Queries;

public record GetOrdersForWeekQuery(string SessionCookie, MenuForWeekId WeekId)
    : IRequest<ErrorOr<IReadOnlyList<PrimirestFoodOrder>>>;