using MediatR;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Application.Photos.Queries.Waiting;

public record WaitingPhotosQuery() : IRequest<List<Photo>>;