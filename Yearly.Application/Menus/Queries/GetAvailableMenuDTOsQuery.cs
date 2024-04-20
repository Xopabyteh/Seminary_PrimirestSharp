using MediatR;
using Yearly.Contracts.Menu;

namespace Yearly.Application.Menus.Queries;

public class GetAvailableMenuDTOsQuery
    : IRequest<List<WeeklyMenuDTO>>;