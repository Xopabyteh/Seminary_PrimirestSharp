using MediatR;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Foods.Queries
{
    public class GetFoodQueryHandler : IRequestHandler<GetFoodQuery, Food>
    {
        private readonly IFoodRepository _foodRepository;

        public GetFoodQueryHandler(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public Task<Food> Handle(GetFoodQuery request, CancellationToken cancellationToken)
        {
            return _foodRepository.GetFoodByIdAsync(request.Id)!;
        }
    }
}