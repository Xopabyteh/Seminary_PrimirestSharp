using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Domain.Repositories;
public interface IWeeklyMenuRepository
{
    public Task AddMenuAsync(WeeklyMenu weeklyMenu);

    public Task<bool> DoesMenuExist(WeeklyMenuId id);
    public Task<List<WeeklyMenuId>> GetWeeklyMenuIdsAsync();
    public Task<int> ExecuteDeleteMenusAsync(List<WeeklyMenuId> menuIds);
}
