using MediatR;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Menus.Queries;

public record GetAvailableWeeklyMenusQuery() : IRequest<List<WeeklyMenu>>;

public class GetAvailableWeeklyMenusQueryHandler : IRequestHandler<GetAvailableWeeklyMenusQuery, List<WeeklyMenu>>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;

    public GetAvailableWeeklyMenusQueryHandler(IWeeklyMenuRepository weeklyMenuRepository)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
    }

    public Task<List<WeeklyMenu>> Handle(GetAvailableWeeklyMenusQuery request, CancellationToken cancellationToken)
    {
        return _weeklyMenuRepository.GetAvailableMenusAsync();
    }
}