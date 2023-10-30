using MediatR;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Foods.Queries;

public record GetFoodQuery(FoodId Id) : IRequest<Food>;