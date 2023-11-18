using Microsoft.EntityFrameworkCore;
using Yearly.Contracts.Menu;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.Repositories.DTORepositories;

public class WeeklyMenuDTORepository
{
    private readonly PrimirestSharpDbContext _context;

    public WeeklyMenuDTORepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<AvailableMenusResponse> GetAvailableMenusAsync()
    {
        //Todo: make this work...
        var weeklyMenus = await _context
            .WeeklyMenus
            .Select(w => new WeeklyMenuResponse(
                w.DailyMenus.Select(d => new DailyMenuResponse(
                        d.Date,
                        _context.Foods
                            .Where(f => d.FoodIds.Contains(f.Id))
                            .Select(f => new FoodResponse(
                                f.Name,
                                f.Allergens,
                                new(),
                                f.Id.Value,
                                new(f.PrimirestFoodIdentifier.MenuId, f.PrimirestFoodIdentifier.DayId, f.PrimirestFoodIdentifier.ItemId)))
                            .ToList()))
                    .ToList(),
                w.Id.Value))
            .ToListAsync();

        return new AvailableMenusResponse(weeklyMenus);
    }
}