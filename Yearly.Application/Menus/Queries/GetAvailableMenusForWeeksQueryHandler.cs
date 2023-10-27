using MediatR;
using Yearly.Domain.Models.MenuForWeekAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Menus.Queries;

public class GetAvailableMenusForWeeksQueryHandler : IRequestHandler<GetAvailableMenusForWeeksQuery, List<MenuForWeek>>
{
    private readonly IMenuForWeekRepository _menuForWeekRepository;

    public GetAvailableMenusForWeeksQueryHandler(IMenuForWeekRepository menuForWeekRepository)
    {
        _menuForWeekRepository = menuForWeekRepository;
    }

    public Task<List<MenuForWeek>> Handle(GetAvailableMenusForWeeksQuery request, CancellationToken cancellationToken)
    {
        return _menuForWeekRepository.GetAvailableMenusAsync();
    }
}