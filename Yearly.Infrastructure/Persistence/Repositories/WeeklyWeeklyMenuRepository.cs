using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class WeeklyWeeklyMenuRepository : IWeeklyMenuRepository
{
    private readonly PrimirestSharpDbContext _context;

    public WeeklyWeeklyMenuRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task AddMenuAsync(WeeklyMenu weeklyMenu)
    {
        await _context.WeeklyMenus.AddAsync(weeklyMenu);
    }

    public async Task<bool> DoesMenuForWeekExistAsync(WeeklyMenuId id)
    {
        return await _context.WeeklyMenus.AnyAsync(m => m.Id == id);
    }

    public Task<List<WeeklyMenu>> GetAvailableMenusAsync()
    {
        return _context.WeeklyMenus.ToListAsync();
    }
}