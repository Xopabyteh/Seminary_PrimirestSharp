using MediatR;
using Yearly.Contracts.Common;
using Yearly.Contracts.Photos;

namespace Yearly.Application.Photos.Queries
{
    public record GetPhotosWithContextDataFragmentQuery(
        int PageOffset,
        int PageSize)
        : IRequest<DataFragmentDTO<PhotoWithContextDTO>>;
}