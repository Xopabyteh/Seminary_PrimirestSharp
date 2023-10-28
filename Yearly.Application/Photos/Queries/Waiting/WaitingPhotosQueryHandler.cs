using MediatR;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Queries.Waiting
{
    public class WaitingPhotosQueryHandler : IRequestHandler<WaitingPhotosQuery, List<Photo>>
    {
        private readonly IPhotoRepository _photoRepository;

        public WaitingPhotosQueryHandler(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public Task<List<Photo>> Handle(WaitingPhotosQuery request, CancellationToken cancellationToken)
        {
            return _photoRepository.GetWaitingPhotosAsync();
        }
    }
}