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

        public Task<List<Photo>> Handle(PhotosForFoodQuery request, CancellationToken cancellationToken)
        {
            return _photoRepository.GetApprovedPhotosForFoodAsync(request.Id);
        }
    }
}