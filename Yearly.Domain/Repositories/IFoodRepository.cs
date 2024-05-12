
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Repositories;

public interface IFoodRepository
{
    public Task<Food?> GetFoodByIdAsync(FoodId id);
    /// <summary>
    /// Key: PrimirestItemId, Value: Food
    /// </summary>
    /// <param name="itemIds"></param>
    /// <returns></returns>
    public Task<Dictionary<int, Food>> GetFoodsByPrimirestItemIdsAsync(List<int> itemIds);
    public Task UpdateFoodAsync(Food food);
    public Task<List<PrimirestFoodIdentifier>> GetFoodsWithIdentifiersThatAlreadyExistAsync(
        List<PrimirestFoodIdentifier> identifiers);
    public Task AddFoodsAsync(List<Food> foods);
}