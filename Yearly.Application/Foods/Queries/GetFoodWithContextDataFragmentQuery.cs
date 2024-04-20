using MediatR;
using Yearly.Contracts.Common;
using Yearly.Contracts.Foods;

namespace Yearly.Application.Foods.Queries;

public record GetFoodWithContextDataFragmentQuery(
    FoodsWithContextFilter Filter,
    int PageOffset,
    int PageSize)
    : IRequest<DataFragmentDTO<FoodWithContextDTO>>;