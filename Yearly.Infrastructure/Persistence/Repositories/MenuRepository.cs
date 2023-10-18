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
}