using MediatR;
using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Application.Foods.Queries;

public record GetFoodQuery(FoodId Id) : IRequest<Food>;