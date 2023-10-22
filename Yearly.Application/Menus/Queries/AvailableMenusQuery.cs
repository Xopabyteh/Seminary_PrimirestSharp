using ErrorOr;
using MediatR;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Application.Menus.Queries;

public record AvailableMenusQuery : IRequest<List<Menu>>;