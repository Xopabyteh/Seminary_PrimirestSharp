using MediatR;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Application.Foods.Queries;

public record FoodsForMenusQuery(List<Menu> Menus)
    : IRequest<List<Food>>;
