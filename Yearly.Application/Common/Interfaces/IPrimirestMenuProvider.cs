using ErrorOr;
using Yearly.Application.Menus;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestMenuProvider
{
    public Task<ErrorOr<List<PrimirestMenuForWeek>>> GetMenusThisWeekAsync();
}