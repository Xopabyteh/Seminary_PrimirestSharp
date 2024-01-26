using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class FoodRepository : IFoodRepository
{
    private readonly PrimirestSharpDbContext _context;

    public FoodRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns food as tracking
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Food?> GetFoodByIdAsync(FoodId id)
    {
        return await _context.Foods.AsTracking().SingleOrDefaultAsync(f => f.Id == id);
    }

    //Key: PrimirestItemId, Value: Food
    public async Task<Dictionary<int, Food>> GetFoodsByPrimirestItemIdsAsync(List<int> itemIds)
    {
        var foods = await _context.Foods
            .Where(f => itemIds.Contains(f.PrimirestFoodIdentifier.ItemId))
            .ToDictionaryAsync(f => f.PrimirestFoodIdentifier.ItemId);
        return foods;
    }

    //Todo: wtf
    public Task UpdateFoodAsync(Food food)
    {
        _context.Foods.Update(food);
        return Task.CompletedTask;
    }

    internal async Task<bool> DoesFoodWithPrimirestIdentifierExistAsync(PrimirestFoodIdentifier identifier)
    {
        var result = await _context.Foods.AnyAsync(f => f.PrimirestFoodIdentifier == identifier);
        return result;
    }

    internal async Task AddFoodAsync(Food food)
    {
        await _context.Foods.AddAsync(food);
    }
}