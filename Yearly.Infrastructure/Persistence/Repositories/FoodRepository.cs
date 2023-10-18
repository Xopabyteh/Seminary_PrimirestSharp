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

    public async Task<Food?> GetFoodByIdAsync(FoodId id)
    {
        return await _context.Foods.FindAsync(id);
    }

    public Task<Food?> GetFoodByNameAsync(string foodName)
    {
        return _context.Foods.SingleOrDefaultAsync(f => f.Name == foodName);
    }

    public async Task<bool> DoesFoodExistAsync(string foodName)
    {
        var exists = await _context.Foods.AnyAsync(f => f.Name == foodName);
        return exists;
    }
}