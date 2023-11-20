using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Persistence;

namespace Yearly.Infrastructure.Services.Foods;

public class FoodSimilarityService : IFoodSimilarityService
{
    private readonly PrimirestSharpDbContext _dbContext;

    public FoodSimilarityService(PrimirestSharpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Unit>> AddToSimilarityTableAsync(List<Food> newlyPersistedFoods)
    {
        if(newlyPersistedFoods.Count == 0)
            return Unit.Value;

        // Load foods from db (only name and id, because we don't want to kill the memory)
        // We also don't care about foods, that already have an alias
        var viewsFromDb = await _dbContext.Foods
            .Where(f => f.AliasForFoodId == null)
            .Select(f => new FoodView(f.Id, f.Name))
            .ToListAsync();

        // Map foods to food views
        var newlyPersistedFoodViews = newlyPersistedFoods
            .Select(f => new FoodView(f.Id, f.Name))
            .ToList();

        var similarityAlg = new F23.StringSimilarity.JaroWinkler();
        var similarityTable = new List<FoodSimilarityRecord>(
            (newlyPersistedFoods.Count * newlyPersistedFoods.Count) + (newlyPersistedFoods.Count * viewsFromDb.Count));

        for (int f = 0; f < newlyPersistedFoods.Count; f++)
        {
            var firstFood = newlyPersistedFoodViews[f];

            //Compare to other new foods
            for (int s = f + 1; s < newlyPersistedFoods.Count; s++)
            {
                var secondFood = newlyPersistedFoodViews[s];
                var similarity = similarityAlg.Similarity(firstFood.Name, secondFood.Name);
                similarityTable.Add(new (firstFood.Id, secondFood.Id, similarity));
            }

            //Compare to foods from db
            for (int s = 0; s < viewsFromDb.Count; s++)
            {
                var secondFood = viewsFromDb[s];
                var similarity = similarityAlg.Similarity(firstFood.Name, secondFood.Name);
                similarityTable.Add(new (firstFood.Id, secondFood.Id, similarity));
            }
        }

        const float similarityThreshold = 0.8f; //If the foods are under this threshold, don't bother counting them in 
        var similarFoods = similarityTable
            .Where(r => r.Similarity >= similarityThreshold);

        await _dbContext.FoodSimilarityTable.AddRangeAsync(similarFoods);

        return Unit.Value;
    }

    public async Task<List<FoodSimilarityRecord>> GetSimilarityTableAsync()
    {
        return await _dbContext.FoodSimilarityTable.ToListAsync();
    }

    public void RemoveRecordFromTable(FoodId newlyPersistedFoodId, FoodId potentialAliasOriginId)
    {
        var record = new FoodSimilarityRecord(newlyPersistedFoodId, potentialAliasOriginId, 0);
        _dbContext.FoodSimilarityTable.Remove(record);
    }


    private readonly record struct FoodView(FoodId Id, string Name);
}