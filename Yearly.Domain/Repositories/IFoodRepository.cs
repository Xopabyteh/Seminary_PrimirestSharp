using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Domain.Repositories
{
    public interface IFoodRepository
    {
        public Task AddFoodAsync(Food food);
        public Task<Food?> GetFoodByIdAsync(FoodId id);
        public Task<Food?> GetFoodByNameAsync(string foodName);
        public Task<bool> DoesFoodExistAsync(string foodName);
        public Task<List<Food>> GetFoodsForMenusAsync(List<Menu> menus);
    }
}