using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Repositories;

public interface IFoodRepository
{
    public Task AddFoodAsync(Food food);
    public Task<Food?> GetFoodByIdAsync(FoodId id);
    public Task<Food?> GetFoodByNameAsync(string foodName);
    public Task<Dictionary<int, Food>> GetFoodsByPrimirestItemIdsAsync(List<int> itemIds);
    public Task UpdateFoodAsync(Food food);
}