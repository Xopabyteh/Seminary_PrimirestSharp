using Microsoft.EntityFrameworkCore;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Infrastructure.Persistence.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly PrimirestSharpDbContext _context;

    public PhotoRepository(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Photo photo)
    {
        await _context.Photos.AddAsync(photo);
    }

    /// <summary>
    /// Returns the photo as tracking
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Photo?> GetAsync(PhotoId id)
    {
        return _context.Photos.AsTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Photo>> GetApprovedPhotosForFoodAsync(FoodId foodId)
    {
        return await _context.Photos.Where(p => p.FoodId == foodId && p.IsApproved).ToListAsync();
    }

    public async Task<List<Photo>> GetWaitingPhotosAsync()
    {
        return await _context.Photos.Where(p => p.IsApproved == false).ToListAsync();
    }

    public Task UpdatePhotoAsync(Photo photo)
    {
        _context.Photos.Update(photo);
        return Task.CompletedTask;
    }

    public Task DeletePhotoAsync(Photo photo)
    {
        _context.Photos.Remove(photo);
        return Task.CompletedTask;
    }

}