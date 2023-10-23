using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Menus.Commands;

/// <summary>
/// Get's menus from primirest and persists foods and menus for those foods into our db.
/// TODO: Updates order identifiers for foods if they already exist.
/// </summary>
public class PersistAvailableMenusCommandHandler : IRequestHandler<PersistAvailableMenusCommand, ErrorOr<Unit>>
{
    private readonly IPrimirestMenuProvider _primirestMenuProvider;
    private readonly IMenuRepository _menuRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly ILogger<PersistAvailableMenusCommandHandler> _logger;

    public PersistAvailableMenusCommandHandler(
        IPrimirestMenuProvider primirestMenuProvider,
        IMenuRepository menuRepository,
        IUnitOfWork unitOfWork,
        IFoodRepository foodRepository,
        IAuthService authService,
        ILogger<PersistAvailableMenusCommandHandler> logger)
    {
        _primirestMenuProvider = primirestMenuProvider;
        _menuRepository = menuRepository;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
        _authService = authService;
        _logger = logger;
    }

    public async Task<ErrorOr<Unit>> Handle(PersistAvailableMenusCommand request, CancellationToken cancellationToken)
    {
        //Auth: Only admins can do this
        var userResult = await _authService.GetSharpUser(request.SessionCookie);
        if (userResult.IsError)
            return userResult.Errors;

        if (!userResult.Value.Roles.Contains(UserRole.Admin))
            return Errors.Errors.Authentication.InsufficientPermissions;

        // Load menu from primirest
        // Persist menu

        var primirestMenusResult = await _primirestMenuProvider.GetMenusThisWeekAsync();
        if (primirestMenusResult.IsError)
            return primirestMenusResult.Errors;

        var primirestMenusForDay = primirestMenusResult.Value;
        if (primirestMenusForDay.Count == 0)
            return Unit.Value; //No menus available -> nothing to persist

        foreach (var primirestMenuForDay in primirestMenusForDay)
        {
            //If we already have this menu, skip it
            if (await _menuRepository.DoesMenuExistForDateAsync(primirestMenuForDay.Date))
                continue;

            var foodIdsForMenu = new List<FoodId>(4);

            //Get foods from our repository
            foreach (var primirestFood in primirestMenuForDay.Foods)
            {
                var food = await _foodRepository.GetFoodByNameAsync(primirestFood.Name);
                if (food is null) //If we don't have food yet, create it
                {
                    food = Food.Create(primirestFood.Name, primirestFood.Allergens, primirestFood.PrimirestOrderIdentifier);
                    _logger.Log(LogLevel.Information, "New food created - {foodName}", food.Name);
                    await _foodRepository.AddFoodAsync(food);
                }
                else
                {
                    //TODO: Update primirest order identifier if we already have the food
                }


                foodIdsForMenu.Add(food.Id);
            }

            //Create menu
            var menu = Menu.Create(foodIdsForMenu, primirestMenuForDay.Date);

            //Persist menu
            await _menuRepository.AddMenuAsync(menu);
        }

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
