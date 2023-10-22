using ErrorOr;
using MediatR;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Menus.Commands;
public class PersistMenuForThisWeekCommandHandler : IRequestHandler<PersistMenuForThisWeekCommand, ErrorOr<int>>
{
    private readonly IExternalServiceMenuProvider _externalServiceMenuProvider;
    private readonly IMenuRepository _menuRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;


    public PersistMenuForThisWeekCommandHandler(IExternalServiceMenuProvider externalServiceMenuProvider, IMenuRepository menuRepository, IUnitOfWork unitOfWork, IFoodRepository foodRepository, IAuthService authService)
    {
        _externalServiceMenuProvider = externalServiceMenuProvider;
        _menuRepository = menuRepository;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
        _authService = authService;
    }

    public async Task<ErrorOr<int>> Handle(PersistMenuForThisWeekCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _authService.GetSharpUser(request.SessionCookie);
        if(userResult.IsError)
            return userResult.Errors;

        if(!userResult.Value.Roles.Contains(UserRole.Admin))
            return Errors.Errors.Authentication.InsufficientPermissions;

        // Load menu from external
        // Persist menu

        var externalMenusResult = await _externalServiceMenuProvider.GetMenusThisWeekAsync();
        if(externalMenusResult.IsError)
            return externalMenusResult.Errors;

        var externalMenus = externalMenusResult.Value;
        if (externalMenus.Count == 0)
            return Errors.Errors.Menu.NoExternalMenusForThisWeek;

        var addedMenus = 0;

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
            addedMenus++;
        }

        await _unitOfWork.SaveChangesAsync();

        return addedMenus;
    }
}
