using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Domain.Repositories;
public interface IWeeklyMenuRepository
{
    public Task AddMenuAsync(WeeklyMenu weeklyMenu);
    //public Task<bool> DoesMenuExistForDateAsync(DateTime date);

    public Task<bool> DoesMenuExist(WeeklyMenuId id);

    /// <summary>
    /// Get's all the menus that are available since the given date.
    /// </summary>
    /// <param name="inclusiveDate"></param>
    /// <returns></returns>
    public Task<List<WeeklyMenu>> GetAvailableMenusAsync();

    /// <summary>
    /// Deletes all menus before the given date.
    /// </summary>
    /// <param name="exclusiveDate"></param>
    /// <returns>Count of deleted instances</returns>
    public Task<int> ExecuteDeleteMenusBeforeDateAsync(DateTime exclusiveDate);
}
