using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Menus.Commands;
public class PersistMenuForThisWeekCommandHandler : IRequestHandler<PersistMenuForThisWeekCommand, ErrorOr<Unit>>
{
    private readonly IExternalServiceMenuProvider _externalServiceMenuProvider;
    private readonly IMenuRepository _menuRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PersistMenuForThisWeekCommandHandler(IExternalServiceMenuProvider externalServiceMenuProvider, IMenuRepository menuRepository, IUnitOfWork unitOfWork, IFoodRepository foodRepository)
    {
        _externalServiceMenuProvider = externalServiceMenuProvider;
        _menuRepository = menuRepository;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(PersistMenuForThisWeekCommand request, CancellationToken cancellationToken)
    {
        // Load menu from external
        // Persist menu

        var externalMenusResult = await _externalServiceMenuProvider.GetMenusThisWeekAsync();
        if(externalMenusResult.IsError)
            return externalMenusResult.Errors;

        var externalMenus = externalMenusResult.Value;
        if (externalMenus.Count == 0)
            return Errors.Errors.Menu.NoExternalMenusForThisWeek;

        foreach (var externalMenu in externalMenus)
        {
            //If we already have this menu, skip it
            if (await _menuRepository.DoesMenuExistForDateAsync(externalMenu.Date))
                continue;

            var foodIdsForMenu = new List<FoodId>(4);

            //Get foods from our repository
            foreach (var externalFood in externalMenu.Foods) //Todo: make soup into food
            {
                var food = await _foodRepository.GetFoodByNameAsync(externalFood.Name);
                if (food is null)
                    return Errors.Errors.Food.FoodNotFound; //Occurs when we haven't persisted foods from external service yet

                foodIdsForMenu.Add(food.Id);
            }

            //Create menu
            var menu = Menu.Create(foodIdsForMenu, externalMenu.Date);

            //Persist menu
            await _menuRepository.AddMenuAsync(menu);
        }

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
