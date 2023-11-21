using Microsoft.EntityFrameworkCore;
using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;

namespace Yearly.Infrastructure.Persistence.Repositories.DTORepositories;

public class WeeklyMenuDTORepository
{
    private readonly PrimirestSharpDbContext _context;

    public WeeklyMenuDTORepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<AvailableMenusResponse> GetAvailableMenus()
    {
        //Todo: make this work...
        var weeklyMenus = await _context
            .WeeklyMenus
            .AsSplitQuery()
            .Select(w => new WeeklyMenuDTO(
                w.DailyMenus.Select(d => new DailyMenuDTO(
                        d.Date,
                        _context.Foods
                            .Where(f => d.Foods.Any(dFId => dFId.FoodId == f.Id))
                            .Select(f => new FoodDTO(
                                f.Name,
                                f.Allergens,
                                _context.Photos
                                    .Where(p => p.FoodId == f.Id)
                                    .Select(p => p.Link)
                                    .ToList(),
                                f.Id.Value,
                                new(f.PrimirestFoodIdentifier.MenuId, f.PrimirestFoodIdentifier.DayId, f.PrimirestFoodIdentifier.ItemId)))
                            .ToList()))
                    .ToList(),
                w.Id.Value))
            .ToListAsync();

        return new AvailableMenusResponse(weeklyMenus);
    }
}