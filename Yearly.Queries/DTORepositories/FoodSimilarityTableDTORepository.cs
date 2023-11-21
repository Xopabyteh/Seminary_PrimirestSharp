using Microsoft.EntityFrameworkCore;
using Yearly.Contracts.Common;
using Yearly.Contracts.Foods;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Persistence;

namespace Yearly.Queries.DTORepositories;

public class FoodSimilarityTableDTORepository
{
    private readonly PrimirestSharpDbContext _context;

    public FoodSimilarityTableDTORepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<List<FoodSimilarityRecordDTO>> GetFoodSimilarityTableAsync()
    {
        var foodSimilarityRecords =
            await _context.FoodSimilarityTable.Select(r => new FoodSimilarityRecordDTO(
                _context.Foods.Where(f => f.Id == r.NewlyPersistedFoodId)
                    .Select(f => new FoodSimilarityRecordSliceDTO(f.Name, f.Id.Value)).First(),
                _context.Foods.Where(f => f.Id == r.PotentialAliasOriginId)
                    .Select(f => new FoodSimilarityRecordSliceDTO(f.Name, f.Id.Value)).First(),
                r.Similarity)).ToListAsync();

        return foodSimilarityRecords;
    }
}