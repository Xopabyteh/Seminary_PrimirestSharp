using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Persistence;

namespace Yearly.Infrastructure.Services.Foods;

public class FoodSimilarityService : IFoodSimilarityService
{
    private readonly PrimirestSharpDbContext _dbContext;
    private readonly FoodSimilarityServiceOptions _options;

    public FoodSimilarityService(
        PrimirestSharpDbContext dbContext,
        IOptions<FoodSimilarityServiceOptions> options)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    /// <summary>
    /// Creates similarity records from newly learned foods.
    /// Compares the foods to each other and to foods which are currently in the database.
    /// For this to work properly, the new foods must be saved to the database AFTER this has been calculated,
    /// otherwise there will be more comparisons than needed-
    /// </summary>
    /// <param name="newlyLearnedFoods"></param>
    /// <returns></returns>
    public async Task<ErrorOr<Unit>> AddToSimilarityTableAsync(List<Food> newlyLearnedFoods)
    {
        if(newlyLearnedFoods.Count == 0)
            return Unit.Value;

        // Load foods from db (only name and id, because we don't want to kill the memory)
        // We also don't care about foods, that already have an alias
        var viewsFromDb = await _dbContext.Foods
            .AsNoTracking()
            .Where(f => f.AliasForFoodId == null)
            .Select(f => new FoodView(f.Id, f.Name))
            .ToListAsync();

        // Map newly learned foods to food views
        var newlyLearnedFoodViews = newlyLearnedFoods
            .Select(f => new FoodView(f.Id, f.Name))
            .ToList();

        var similarityAlg = new F23.StringSimilarity.JaroWinkler();
        var similarFoods = new List<FoodSimilarityRecord>();

        //Compare newly learned foods with each other, and aginst foods in database
        for (int f = 0; f < newlyLearnedFoods.Count; f++)
        {
            var firstFood = newlyLearnedFoodViews[f];

            //Compare to other new foods
            for (int s = f + 1; s < newlyLearnedFoods.Count; s++)
            {
                var secondFood = newlyLearnedFoodViews[s];
                var similarity = similarityAlg.Similarity(firstFood.Name, secondFood.Name);
                if (similarity >= _options.NameStringSimilarityThreshold)
                {
                    similarFoods.Add(new (firstFood.Id, secondFood.Id, similarity));
                }
            }

            //Compare to foods from db
            for (int s = 0; s < viewsFromDb.Count; s++)
            {
                var secondFood = viewsFromDb[s];
                var similarity = similarityAlg.Similarity(firstFood.Name, secondFood.Name);
                if (similarity >= _options.NameStringSimilarityThreshold)
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
            .ToListAsync();

        var updatedFoods = new List<Food>();
        foreach (var foodSimilarityRecord in identicalFoodRecords)
        {
            var newFood = await _dbContext.Foods
                .AsTracking()
                .FirstAsync(f=> f.Id == foodSimilarityRecord.NewlyPersistedFoodId);

            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
            var potentialAlias = await _dbContext.Foods
                .AsNoTracking()
                .FirstAsync(p => p.Id == foodSimilarityRecord.PotentialAliasOriginId);

            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
            if (potentialAlias.AliasForFoodId is not null)
            {
                //Drop the record, the food became an alias in this process
                //We don't want to have an alias for a food with an alias already
                //We want to find the root food
                continue;
            }
            newFood.SetAliasForFood(potentialAlias);
            updatedFoods.Add(newFood);
        }

        _dbContext.Foods.UpdateRange(updatedFoods);

        await _dbContext.FoodSimilarityTable
            .Where(r => r.Similarity >= 1)
            .ExecuteDeleteAsync();
    }

    private readonly record struct FoodView(FoodId Id, string Name);
}