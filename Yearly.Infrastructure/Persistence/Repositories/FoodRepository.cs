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

    public async Task AddFoodAsync(Food food)
    {
        await _context.Foods.AddAsync(food);
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

    public Task<Food?> GetFoodByNameAsync(string foodName)
    {
        return _context.Foods.SingleOrDefaultAsync(f => f.Name == foodName);
    }

    //Key: PrimirestItemId, Value: Food
    public Task<Dictionary<int, Food>> GetFoodsByPrimirestItemIdsAsync(List<int> itemIds)
    {
        var foods = _context.Foods
            .Where(f => itemIds.Contains(f.PrimirestFoodIdentifier.ItemId))
            .ToDictionaryAsync(f => f.PrimirestFoodIdentifier.ItemId);
        return foods;
    }

    public Task UpdateFoodAsync(Food food)
    {
        _context.Foods.Update(food);
        return Task.CompletedTask;
    }
}