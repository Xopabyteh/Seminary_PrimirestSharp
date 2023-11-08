using Microsoft.EntityFrameworkCore;
using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.Repositories.QueryRepositories;

public class WeeklyMenuQueryRepository
{
    private readonly PrimirestSharpDbContext _context;

    public WeeklyMenuQueryRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<AvailableMenusResponse> GetAvailableMenusDtoAsync()
    {
        //Todo: DDD Identity paradox

  var weeklyMenuResponses = await _context
            .WeeklyMenus
            .Select(w => new WeeklyMenuResponse(
                w.DailyMenus
                    .Select(d => new DailyMenuResponse(
                        d.Date,
                        _context.Foods
                            .Where(f => d.FoodIds.Contains(f.Id))
                            //.Where(f => d.FoodIds.Contains(new FoodId(Guid.Empty)))
                            .Select(f => new FoodResponse(
                                f.Name,
                                f.Allergens,
                                //_context.Photos
                                //    .Where(p => f.PhotoIds.Contains(p.Id))
                                //    .Select(p => p.Link)
                                //    .ToList(),
                                new List<string>(),
                                f.Id.Value,
                                new PrimirestFoodIdentifierContract(
                                    f.PrimirestFoodIdentifier.MenuId,
                                    f.PrimirestFoodIdentifier.DayId,
                                    f.PrimirestFoodIdentifier.ItemId)))
                            .ToList()))
                    .ToList(),
                w.Id.Value))
            .ToListAsync();

        return new AvailableMenusResponse(weeklyMenuResponses);
    }
}