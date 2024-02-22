using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class WeeklyMenuRepository : IWeeklyMenuRepository
{
    private readonly PrimirestSharpDbContext _context;

    public WeeklyMenuRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    //public async Task AddMenuAsync(WeeklyMenu weeklyMenu)
    //{
    //    await _context.WeeklyMenus.AddAsync(weeklyMenu);
    //}

    public async Task<bool> DoesMenuExist(WeeklyMenuId id)
    {
        return await _context.WeeklyMenus.AnyAsync(m => m.Id == id);
    }

    public async Task<List<WeeklyMenuId>> GetWeeklyMenuIdsAsync()
    {
        var ids = await _context.WeeklyMenus.Select(m => m.Id).ToListAsync();
        return ids;
    }

    /// <returns>Count of deleted menus</returns>
    public async Task<int> ExecuteDeleteMenusAsync(List<WeeklyMenuId> menuIds)
    {
        var deleteCount = await _context
            .WeeklyMenus
            .Where(w => menuIds.Contains(w.Id))
            .ExecuteDeleteAsync();

        return deleteCount;
    }

    public async Task AddMenusAsync(List<WeeklyMenu> menus)
    {
        await _context.WeeklyMenus.AddRangeAsync(menus);
    }
}