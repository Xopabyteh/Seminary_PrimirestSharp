﻿using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Menus.Commands;

public record PersistAvailableMenusCommand : IRequest<ErrorOr<Unit>>;

/// <summary>
/// Get's menus from primirest and persists foods and menus for those foods into our db.
/// </summary>
public class PersistAvailableMenusCommandHandler : IRequestHandler<PersistAvailableMenusCommand, ErrorOr<Unit>>
{
    private readonly IPrimirestMenuPersister _primirestMenuPersister;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFoodSimilarityService _foodSimilarityService;

    public PersistAvailableMenusCommandHandler(
        IPrimirestMenuPersister primirestMenuPersister,
        IUnitOfWork unitOfWork,
        IFoodSimilarityService foodSimilarityService)
    {
        _primirestMenuPersister = primirestMenuPersister;
        _unitOfWork = unitOfWork;
        _foodSimilarityService = foodSimilarityService;
    }

    public async Task<ErrorOr<Unit>> Handle(PersistAvailableMenusCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await _primirestMenuPersister.DeleteOldMenusAsync();
        if(deleteResult.IsError)
            return deleteResult.Errors;

        //The menus here are not yet in the db, because unit of work was not saved yet.
        var newlyPersistedFoods = await _primirestMenuPersister.PersistAvailableMenusAsync();
        if(newlyPersistedFoods.IsError)
            return newlyPersistedFoods.Errors;

        //Create similarity table
        if (newlyPersistedFoods.Value.Count > 0)
        {
            await _foodSimilarityService.AddToSimilarityTableAsync(newlyPersistedFoods.Value);
        }

        await _unitOfWork.SaveChangesAsync();

        await _foodSimilarityService.AutoAliasIdenticalFoodsAsync(); //Todo: might move to an event

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
