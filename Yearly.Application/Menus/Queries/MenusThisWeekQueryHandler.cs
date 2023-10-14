using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Application.Menus.Queries;

public class MenusThisWeekQueryHandler : IRequestHandler<MenusThisWeekQuery, ErrorOr<List<Menu>>>
{
    private readonly IExternalServiceMenuProvider _externalServiceMenuProvider;

    public MenusThisWeekQueryHandler(IExternalServiceMenuProvider externalServiceMenuProvider)
    {
        _externalServiceMenuProvider = externalServiceMenuProvider;
    }

    public async Task<ErrorOr<List<Menu>>> Handle(MenusThisWeekQuery request, CancellationToken cancellationToken)
    {
        var externalMenusResult = await _externalServiceMenuProvider.GetMenusThisWeekAsync();


        return new List<Menu>();
    }
}