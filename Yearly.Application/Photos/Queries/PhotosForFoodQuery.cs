using MediatR;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Application.Photos.Queries;

public record PhotosForFoodQuery(FoodId Id) : IRequest<List<Photo>>;