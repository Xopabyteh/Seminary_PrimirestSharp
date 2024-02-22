using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

internal sealed class FoodRepository : IFoodRepository
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
    public async Task<bool> DoesFoodWithPrimirestIdentifierExistAsync(PrimirestFoodIdentifier identifier)
    {
        var result = await _context.Foods.AnyAsync(f => f.PrimirestFoodIdentifier == identifier);
        return result;
    }

    ///<returns>A list of foods that answer yes to: "Is there a food in our DB that has the same order identifier?"</returns>
    public async Task<List<PrimirestFoodIdentifier>> GetFoodsWithIdentifiersThatAlreadyExistAsync(
        List<PrimirestFoodIdentifier> identifiers)
    {
        //Todo: Wtf, why does this not work like this? Why must there be this obscure select statement
        //var result = await _context.Foods
        //    .AsNoTracking()
        //    .Where(f => identifiers.Contains(f.PrimirestFoodIdentifier))
        //    .Select(f => f.PrimirestFoodIdentifier)
        //    .ToListAsync();

        var identifierItemIds = identifiers.Select(i => i.ItemId).ToList();
        var result = await _context.Foods
            .AsNoTracking()
            .Where(f => identifierItemIds.Contains(f.PrimirestFoodIdentifier.ItemId))
            .Select(f => f.PrimirestFoodIdentifier)
            .ToListAsync();

        return result;
    }
    //public async Task AddFoodAsync(Food food)
    //{
    //    await _context.Foods.AddAsync(food);
    //}

    public async Task AddFoodsAsync(List<Food> foods)
    {
        await _context.Foods.AddRangeAsync(foods);
    }
}