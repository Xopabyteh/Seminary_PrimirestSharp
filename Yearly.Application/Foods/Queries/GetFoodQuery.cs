using MediatR;
using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Application.Foods.Queries;

public record GetFoodQuery(Guid Id) : IRequest<Food>;