using MediatR;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Application.Photos.Queries.ForFood;

public record PhotosForFoodQuery(Guid Id) : IRequest<List<Photo>>;