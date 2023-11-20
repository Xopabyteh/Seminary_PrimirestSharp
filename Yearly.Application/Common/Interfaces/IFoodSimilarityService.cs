using ErrorOr;
using MediatR;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Common.Interfaces;

public interface IFoodSimilarityService
{
    /// <summary>
    /// Takes the given foods and adds them to the food similarity table.
    /// Compares the foods to each other and to the foods already in the database.
    /// The foods that are given as <see cref="newlyPersistedFoods"/> are expected not to be located in the database yet.
    /// </summary>
    /// <param name="newlyPersistedFoods"></param>
    /// <returns></returns>
    public Task<ErrorOr<Unit>> AddToSimilarityTableAsync(List<Food> newlyPersistedFoods);

    public Task<List<FoodSimilarityRecord>> GetSimilarityTableAsync();

    public void RemoveRecordFromTable(FoodId newlyPersistedFoodId, FoodId potentialAliasOriginId);
}