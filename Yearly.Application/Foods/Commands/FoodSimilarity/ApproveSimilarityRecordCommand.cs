using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Foods.Commands.FoodSimilarity;

/// <summary>
/// Set the alias of the food with id <see cref="OfFood"/> to the food with id <see cref="ForFood"/>.
/// </summary>
/// <param name="OfFood"></param>
/// <param name="ForFood"></param>
public readonly record struct ApproveSimilarityRecordCommand(FoodId OfFood, FoodId ForFood) : IRequest<ErrorOr<Unit>>;

public class ApproveSimilarityRecordCommandHandler : IRequestHandler<ApproveSimilarityRecordCommand, ErrorOr<Unit>>
{
    private readonly IFoodSimilarityService _foodSimilarityService;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveSimilarityRecordCommandHandler(IFoodSimilarityService foodSimilarityService, IFoodRepository foodRepository, IUnitOfWork unitOfWork)
    {
        _foodSimilarityService = foodSimilarityService;
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(ApproveSimilarityRecordCommand request, CancellationToken cancellationToken)
    {
        var ofFood = await _foodRepository.GetFoodByIdAsync(request.OfFood);
        var forFood = await _foodRepository.GetFoodByIdAsync(request.ForFood);

        if (ofFood is null)
            return Errors.Errors.Food.FoodNotFound(request.OfFood);

        if (forFood is null)
            return Errors.Errors.Food.FoodNotFound(request.ForFood);

        ofFood.SetAliasForFood(forFood);

        //Todo: maybe if ofFood had photos, we move them to forFood
        
        await _foodRepository.UpdateFoodAsync(ofFood);
        _foodSimilarityService.RemoveRecordFromTable(request.OfFood, request.ForFood);

        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}
