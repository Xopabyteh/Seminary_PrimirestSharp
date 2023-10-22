using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services;

namespace Yearly.Application.Menus.Queries;

public class AvailableMenusQueryHandler : IRequestHandler<AvailableMenusQuery, List<Menu>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AvailableMenusQueryHandler(IMenuRepository menuRepository, IDateTimeProvider dateTimeProvider)
    {
        _menuRepository = menuRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<List<Menu>> Handle(AvailableMenusQuery request, CancellationToken cancellationToken)
    {
        var menus = await _menuRepository.GetMenusSinceDayAsync(_dateTimeProvider.UtcNow);
        
        return menus; //Just cast from IQueryable to ErrorOr<IQueryable>
    }
}