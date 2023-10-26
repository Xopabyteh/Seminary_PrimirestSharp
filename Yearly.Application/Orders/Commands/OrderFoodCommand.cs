using ErrorOr;
using MediatR;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Orders.Commands;

public record OrderFoodCommand(string SessionCookie, PrimirestOrderIdentifier PrimirestOrderIdentifier)
    : IRequest<ErrorOr<Unit>>;