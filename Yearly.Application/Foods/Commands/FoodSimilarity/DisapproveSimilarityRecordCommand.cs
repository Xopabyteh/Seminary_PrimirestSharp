using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Foods.Commands.FoodSimilarity;

public readonly record struct DisapproveSimilarityRecordCommand(FoodId OfFood, FoodId ForFood) : IRequest;

public class DisapproveSimilarityRecordCommandHandler : IRequestHandler<DisapproveSimilarityRecordCommand>
{
    private readonly IFoodSimilarityService _foodSimilarityService;
    private readonly IUnitOfWork _unitOfWork;

    public DisapproveSimilarityRecordCommandHandler(IFoodSimilarityService foodSimilarityService, IUnitOfWork unitOfWork)
    {
        _foodSimilarityService = foodSimilarityService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DisapproveSimilarityRecordCommand request, CancellationToken cancellationToken)
    {
        _foodSimilarityService.RemoveRecordFromTable(request.OfFood, request.ForFood);
        
        await _unitOfWork.SaveChangesAsync();
    }
}
