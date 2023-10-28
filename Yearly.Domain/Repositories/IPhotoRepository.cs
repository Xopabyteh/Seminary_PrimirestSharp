using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Domain.Repositories;

public interface IPhotoRepository
{
    public Task AddAsync(Photo photo);
    public Task<Photo?> GetAsync(PhotoId id);
    public Task<List<Photo>> GetApprovedPhotosForFoodAsync(FoodId foodId);

    /// <returns>Photos that haven't been approved yet</returns>
    public Task<List<Photo>> GetWaitingPhotosAsync();
    public Task UpdatePhotoAsync(Photo photo);
    public Task DeletePhotoAsync(Photo photo);
}