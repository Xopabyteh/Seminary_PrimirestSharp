using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly PrimirestSharpDbContext _context;

    public PhotoRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task<List<Photo>> GetPhotosForFoodsAsync(List<FoodId> foodIds)
    {
        var photos = await _context.Photos.Where(p => foodIds.Contains(p.FoodId)).ToListAsync();
        return photos;
    }
}