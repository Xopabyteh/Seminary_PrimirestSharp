using ErrorOr;
using Yearly.Application.Menus;

namespace Yearly.Application.Common.Interfaces;

public interface IExternalServiceMenuProvider
{
    public Task<ErrorOr<List<ExternalServiceMenu>>> GetMenusThisWeekAsync();
}