using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Foods.Commands.FoodSimilarity;

public readonly record struct SetFoodAliasCommand(FoodId OfFoodId, FoodId ForFoodId) : IRequest<ErrorOr<Unit>>;

public class SetFoodAliasCommandHandler : IRequestHandler<SetFoodAliasCommand, ErrorOr<Unit>>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetFoodAliasCommandHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork)
    {
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(SetFoodAliasCommand request, CancellationToken cancellationToken)
    {
        var ofFood = await _foodRepository.GetFoodByIdAsync(request.OfFoodId);
        if (ofFood is null)
            return Errors.Errors.Food.FoodNotFound(request.OfFoodId);

        var forFood = await _foodRepository.GetFoodByIdAsync(request.ForFoodId);
        if (forFood is null)
            return Errors.Errors.Food.FoodNotFound(request.ForFoodId);

        ofFood.SetAliasForFood(forFood);

        await _foodRepository.UpdateFoodAsync(ofFood);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
