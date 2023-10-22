using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly PrimirestSharpDbContext _context;

    public MenuRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task AddMenuAsync(Menu menu)
    {
        await _context.AddAsync(menu);
    }

    public async Task<bool> DoesMenuExistForDateAsync(DateTime date)
    {
        return await _context.Menus.AnyAsync(m => m.Date == date);
    }

    public async Task<List<Menu>> GetMenusSinceDayAsync(DateTime inclusiveDate)
    {
        var menus = await _context.Menus.Where(m => m.Date >= inclusiveDate).ToListAsync();
        return menus;
    }

    /// <summary>
    /// Deletes the foods immediately without the need to call SaveChanges
    /// </summary>
    /// <param name="inclusiveDate"></param>
    /// <returns></returns>
    public async Task<int> DeleteMenusBeforeDayAsync(DateTime inclusiveDate)
    {
        return await _context.Menus
            .Where(m => m.Date <= inclusiveDate)
            .ExecuteDeleteAsync();
    }
}