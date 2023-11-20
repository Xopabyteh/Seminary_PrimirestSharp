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

    public async Task AddMenuAsync(WeeklyMenu weeklyMenu)
    {
        await _context.WeeklyMenus.AddAsync(weeklyMenu);
    }

    public async Task<bool> DoesMenuExist(WeeklyMenuId id)
    {
        return await _context.WeeklyMenus.AnyAsync(m => m.Id == id);
    }

    public async Task<List<WeeklyMenu>> GetAvailableMenusAsync()
    {
        return await _context.WeeklyMenus.ToListAsync();
    }

    public async Task<int> ExecuteDeleteMenusBeforeDateAsync(DateTime exclusiveDate)
    {
        var deleted = await _context.WeeklyMenus
            .Where(m => m.DailyMenus.OrderByDescending(d => d.Date).First().Date < exclusiveDate)
            .ExecuteDeleteAsync();
        
        return deleted;
    }
}