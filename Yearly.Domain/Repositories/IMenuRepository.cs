using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Domain.Repositories;
public interface IMenuRepository
{
    public Task AddMenuAsync(Menu menu);
    public Task<bool> DoesMenuExistForDateAsync(DateTime date);

    /// <summary>
    /// Get's all the menus that are available since the given date.
    /// </summary>
    /// <param name="inclusiveDate"></param>
    /// <returns></returns>
    public Task<List<Menu>> GetMenusSinceDayAsync(DateTime inclusiveDate);

    /// <summary>
    /// Deletes all menus before the given date.
    /// </summary>
    /// <param name="inclusiveDate"></param>
    /// <returns>Count of deleted instances</returns>
    public Task<int> DeleteMenusBeforeDayAsync(DateTime inclusiveDate);
}
