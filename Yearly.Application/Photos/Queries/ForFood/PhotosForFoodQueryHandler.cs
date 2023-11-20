using MediatR;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Queries.ForFood
{
    public class PhotosForFoodQueryHandler : IRequestHandler<PhotosForFoodQuery, List<Photo>>
    {
        private readonly IPhotoRepository _photoRepository;

        public PhotosForFoodQueryHandler(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<List<Photo>> Handle(PhotosForFoodQuery request, CancellationToken cancellationToken)
        {
            if (request.Food.AliasForFoodId is null)
            {
                //The root food
                return await _photoRepository.GetApprovedPhotosForFoodAsync(request.Food.Id);
            }
            else //Alias is not null
            {
                //The food is an alias for another food
                //Return photos of the alias
                return await _photoRepository.GetApprovedPhotosForFoodAsync(request.Food.AliasForFoodId);
            }
        }
    }
}