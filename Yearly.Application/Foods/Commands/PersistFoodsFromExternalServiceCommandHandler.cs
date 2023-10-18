using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Foods.Commands;
public class PersistFoodsFromExternalServiceCommandHandler : IRequestHandler<PersistFoodsFromExternalServiceCommand, ErrorOr<int>>
{
    private readonly IExternalServiceMenuProvider _externalServiceMenuProvider;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PersistFoodsFromExternalServiceCommandHandler(IExternalServiceMenuProvider externalServiceMenuProvider, IFoodRepository foodRepository, IUnitOfWork unitOfWork)
    {
        _externalServiceMenuProvider = externalServiceMenuProvider;
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Get foods from external service and store them if they don't exist yet.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ErrorOr<int>> Handle(PersistFoodsFromExternalServiceCommand request, CancellationToken cancellationToken)
    {
        var externalMenusResult = await _externalServiceMenuProvider.GetMenusThisWeekAsync();
        if (externalMenusResult.IsError)
            return externalMenusResult.Errors;

        var externalMenus = externalMenusResult.Value;
        var externalFoods = externalMenus.SelectMany(m => m.Foods).ToList();
        var addedFoods = 0;
        foreach (var externalFood in externalFoods)
        {
            var foodExists = await _foodRepository.DoesFoodExistAsync(externalFood.Name);
            if (foodExists)
                continue;

            //Store food
            var food = Food.Create(externalFood.Name, externalFood.Allergens);
            await _foodRepository.AddFoodAsync(food);
            addedFoods++;
        }

        await _unitOfWork.SaveChangesAsync();

        return addedFoods;
    }
}
