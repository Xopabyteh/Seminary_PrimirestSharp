using MediatR;
using Yearly.Contracts.Photos;

namespace Yearly.Application.Photos.Queries;

public record GetWaitingPhotoDTOsQuery 
    : IRequest<List<PhotoDTO>>;