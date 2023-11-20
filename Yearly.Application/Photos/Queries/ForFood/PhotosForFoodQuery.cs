using MediatR;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Application.Photos.Queries.ForFood;

public record PhotosForFoodQuery(Food Food) : IRequest<List<Photo>>;