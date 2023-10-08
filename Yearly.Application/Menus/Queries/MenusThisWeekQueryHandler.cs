using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Application.Menus.Queries;

public class MenusThisWeekQueryHandler : IRequestHandler<MenusThisWeekQuery, ErrorOr<List<Menu>>>
{
    private readonly IMenuProvider _menuProvider;

    public MenusThisWeekQueryHandler(IMenuProvider menuProvider)
    {
        _menuProvider = menuProvider;
    }

    public Task<ErrorOr<List<Menu>>> Handle(MenusThisWeekQuery request, CancellationToken cancellationToken)
    {
        return _menuProvider.GetMenusThisWeek();
    }
}