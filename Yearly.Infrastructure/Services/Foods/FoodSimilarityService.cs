using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Persistence;

namespace Yearly.Infrastructure.Services.Foods;

public class FoodSimilarityService : IFoodSimilarityService
{
    private readonly PrimirestSharpDbContext _dbContext;
    private const float k_SimilarityThreshold = 0.8f; //If the foods are under this threshold, don't bother counting them in 

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
            .AsNoTracking()
            .Where(f => f.AliasForFoodId == null)
            .Select(f => new FoodView(f.Id, f.Name))
            .ToListAsync();

        // Map foods to food views
        var newlyPersistedFoodViews = newlyPersistedFoods
            .Select(f => new FoodView(f.Id, f.Name))
            .ToList();

        var similarityAlg = new F23.StringSimilarity.JaroWinkler();
        var similarFoods = new List<FoodSimilarityRecord>();

        for (int f = 0; f < newlyPersistedFoods.Count; f++)
        {
            var firstFood = newlyPersistedFoodViews[f];

            //Compare to other new foods
            for (int s = f + 1; s < newlyPersistedFoods.Count; s++)
            {
                var secondFood = newlyPersistedFoodViews[s];
                var similarity = similarityAlg.Similarity(firstFood.Name, secondFood.Name);
                if (similarity >= k_SimilarityThreshold)
                {
                    similarFoods.Add(new (firstFood.Id, secondFood.Id, similarity));
                }
            }

            //Compare to foods from db
            for (int s = 0; s < viewsFromDb.Count; s++)
            {
                var secondFood = viewsFromDb[s];
                var similarity = similarityAlg.Similarity(firstFood.Name, secondFood.Name);
                if (similarity >= k_SimilarityThreshold)
                {
                    similarFoods.Add(new (firstFood.Id, secondFood.Id, similarity));
                }
            }
        }

        await _dbContext.FoodSimilarityTable.AddRangeAsync(similarFoods);

        return Unit.Value;
    }

    public void RemoveRecordFromTable(FoodId newlyPersistedFoodId, FoodId potentialAliasOriginId)
    {
        var record = new FoodSimilarityRecord(newlyPersistedFoodId, potentialAliasOriginId, 0);
        _dbContext.FoodSimilarityTable.Remove(record);
    }

    public async Task AutoAliasIdenticalFoodsAsync()
    {
        var identicalFoodRecords = await _dbContext.FoodSimilarityTable
            .Where(r => r.Similarity >= 1)
            //.Include(foodSimilarityRecord => foodSimilarityRecord.NewlyPersistedFoodId)
            //.Include(foodSimilarityRecord => foodSimilarityRecord.PotentialAliasOriginId)
            .ToListAsync();

        var updatedFoods = new List<Food>();
        foreach (var foodSimilarityRecord in identicalFoodRecords)
        {
            var food = await _dbContext.Foods.AsTracking().FirstAsync(f=> f.Id == foodSimilarityRecord.NewlyPersistedFoodId);
            var potentialAlias = await _dbContext.Foods.FindAsync(foodSimilarityRecord.PotentialAliasOriginId);

            if (potentialAlias!.AliasForFoodId is not null)
            {
                //Drop the record, the food became an alias in this process
                //We don't want to have an alias for a food with an alias already
                //We want to find the root food
                continue;
            }
            food!.SetAliasForFood(potentialAlias!);
            updatedFoods.Add(food);
        }

        _dbContext.Foods.UpdateRange(updatedFoods);

        await _dbContext.FoodSimilarityTable
            .Where(r => r.Similarity >= 1)
            .ExecuteDeleteAsync();
    }

    private readonly record struct FoodView(FoodId Id, string Name);
}