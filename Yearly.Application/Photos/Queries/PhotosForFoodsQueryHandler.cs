using MediatR;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Queries;

public class PhotosForFoodsQueryHandler : IRequestHandler<PhotosForFoodsQuery, List<Photo>>
{
    private readonly IPhotoRepository _photoRepository;

    public PhotosForFoodsQueryHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public Task<List<Photo>> Handle(PhotosForFoodsQuery request, CancellationToken cancellationToken)
    {
        var foodIds = request.Foods.Select(f => f.Id).ToList();
        return _photoRepository.GetPhotosForFoodsAsync(foodIds);
    }
}