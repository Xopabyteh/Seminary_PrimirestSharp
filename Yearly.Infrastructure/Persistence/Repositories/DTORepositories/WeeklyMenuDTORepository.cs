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
        //var weeklyMenus = await _context
        //    .WeeklyMenus
        //    .Select(w => new WeeklyMenuResponse(
        //        w.DailyMenus.Select(d => new DailyMenuResponse(
        //            d.Date,
        //            _context.Foods
        //                .Where(f => d.FoodIds.Any(dFId => dFId == f.Id))
        //                .Select(f => new FoodResponse(
        //                    "",
        //                    "",
        //                    new(),
        //                    Guid.Empty,
        //                    new(0, 0, 0)))
        //                .ToList()))
        //            .ToList(),
        //        w.Id))
        //    .ToListAsync();

        var weeklyMenus = await _context.WeeklyMenus
            .Select(w =>
                new WeeklyMenuResponse(
                    w.DailyMenus
                        .Select(d => new DailyMenuResponse(
                            d.Date,
                            d.Foods
                                .Select(f => new FoodResponse(f.Name, f.Allergens, new List<string>(), f.Id, new PrimirestFoodIdentifierContract(0, 0, 0)))
                                .ToList()))
                        .ToList(),
                    w.Id))
            .ToListAsync();

        //var weeklyMenus = await _context.WeeklyMenus
        //    .Select(w =>
        //        new WeeklyMenuResponse(
        //            w.DailyMenus
        //                .Select(d => new DailyMenuResponse(
        //                    d.Date,
        //                    _context.Foods.Where(f => d.FoodIds.Any(dFId => dFId == f.Id))
        //                        .Select(f => new FoodResponse(f.Name, f.Allergens, new List<string>(), f.Id, new PrimirestFoodIdentifierContract(0, 0, 0)))
        //                        .ToList()))
        //                .ToList(),
        //            w.Id))
        //    .ToListAsync();

        return new AvailableMenusResponse(weeklyMenus);
    }
}