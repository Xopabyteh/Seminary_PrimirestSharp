using MediatR;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Foods.Queries;

public class FoodsForMenusQueryHandler : IRequestHandler<FoodsForMenusQuery, List<Food>>
{
    private readonly IFoodRepository _foodRepository;

    public FoodsForMenusQueryHandler(IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<List<Food>> Handle(FoodsForMenusQuery request, CancellationToken cancellationToken)
    {
        var foods = await _foodRepository.GetFoodsByIdsAsync(request.Menus.SelectMany(m => m.FoodIds).ToList());
        return foods;
    }
}