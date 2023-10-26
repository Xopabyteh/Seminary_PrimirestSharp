using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Domain.Repositories;

public interface IPhotoRepository
{
    public Task<List<Photo>> GetPhotosByFoodIdsAsync(List<FoodId> foodIds);

    //public Task<List<Photo>> GetPhotosForSoupsAsync(List<FoodId> FoodIds);
}