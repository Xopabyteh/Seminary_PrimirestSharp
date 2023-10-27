using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.MenuForWeekAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class MenuForWeekRepository : IMenuForWeekRepository
{
    private readonly PrimirestSharpDbContext _context;

    public MenuForWeekRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task AddMenuAsync(MenuForWeek menu)
    {
        await _context.MenusForWeeks.AddAsync(menu);
    }

    public async Task<bool> DoesMenuForWeekExistAsync(PrimirestMenuForWeekId id)
    {
        return await _context.MenusForWeeks.AnyAsync(m => m.Id == id);
    }
}