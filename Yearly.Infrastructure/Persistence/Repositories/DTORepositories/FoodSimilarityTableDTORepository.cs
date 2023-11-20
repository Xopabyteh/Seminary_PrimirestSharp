using Microsoft.EntityFrameworkCore;
using Yearly.Contracts.Common;
using Yearly.Contracts.Foods;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Persistence.Repositories.DTORepositories;

public class FoodSimilarityTableDTORepository
{
    private readonly PrimirestSharpDbContext _context;

    public FoodSimilarityTableDTORepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<List<FoodSimilarityRecordResponse>> GetFoodSimilarityTableAsync()
    {
        var foodSimilarityRecords =
            await _context.FoodSimilarityTable.Select(r => new FoodSimilarityRecordResponse(
                _context.Foods.Where(f => f.Id == r.NewlyPersistedFoodId)
                    .Select(f => new FoodSimilarityRecordSliceResponse(f.Name, f.Id.Value)).First(),
                _context.Foods.Where(f => f.Id == r.PotentialAliasOriginId)
                    .Select(f => new FoodSimilarityRecordSliceResponse(f.Name, f.Id.Value)).First(),
                r.Similarity)).ToListAsync();

        return foodSimilarityRecords;
    }
}