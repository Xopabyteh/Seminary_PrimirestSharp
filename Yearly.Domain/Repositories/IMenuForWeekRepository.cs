using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.MenuForWeekAgg;

namespace Yearly.Domain.Repositories;
public interface IMenuForWeekRepository
{
    public Task AddMenuAsync(MenuForWeek menu);
    //public Task<bool> DoesMenuExistForDateAsync(DateTime date);

    public Task<bool> DoesMenuForWeekExistAsync(PrimirestMenuForWeekId id);

    ///// <summary>
    ///// Get's all the menus that are available since the given date.
    ///// </summary>
    ///// <param name="inclusiveDate"></param>
    ///// <returns></returns>
    //public Task<List<Menu>> GetMenusSinceDayAsync(DateTime inclusiveDate);

    ///// <summary>
    ///// Deletes all menus before the given date.
    ///// </summary>
    ///// <param name="inclusiveDate"></param>
    ///// <returns>Count of deleted instances</returns>
    //public Task<int> DeleteMenusBeforeDayAsync(DateTime inclusiveDate);
}
