using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Domain.Repositories;

public interface IPhotoRepository
{
    public Task AddAsync(Photo photo);
    public Task<Photo?> GetAsync(Guid id);
    public Task<List<Photo>> GetApprovedPhotosForFoodAsync(Guid foodId);

    /// <returns>Photos that haven't been approved yet</returns>
    public Task<List<Photo>> GetWaitingPhotosAsync();
    public Task UpdatePhotoAsync(Photo photo);
    public Task DeletePhotoAsync(Photo photo);
}