using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.MenuForWeekAgg;
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
    private readonly IMenuForWeekRepository _menuRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly ILogger<PersistAvailableMenusCommandHandler> _logger;

    public PersistAvailableMenusCommandHandler(
        IPrimirestMenuProvider primirestMenuProvider,
        IMenuForWeekRepository menuRepository,
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

        var primirestMenusForWeek = primirestMenusResult.Value;
        if (primirestMenusForWeek.Count == 0)
            return Unit.Value; //No menus available -> nothing to persist

        foreach (var primirestMenuForWeek in primirestMenusForWeek)
        {
            var menuForWeekId = new MenuForWeekId(primirestMenuForWeek.PrimirestMenuId);

            //If we already have this menu, skip it
            if (await _menuRepository.DoesMenuForWeekExistAsync(menuForWeekId))
                continue;

            var menusForDays = new List<MenuForDay>(5);

            //Handle foods from this week menu
            foreach (var primirestMenuForDay in primirestMenuForWeek.MenusForDay)
            {
                var foodIdsForDay = new List<FoodId>(3);

                //Handle foods
                foreach (var primirestFood in primirestMenuForDay.Foods)
                {
                    var food = await _foodRepository.GetFoodByNameAsync(primirestFood.Name);
                    if (food is null) //If we don't have food yet, create it
                    {
                        food = Food.Create(primirestFood.Name, primirestFood.Allergens, primirestFood.PrimirestFoodIdentifier);
                        _logger.Log(LogLevel.Information, "New food created - {foodName}", food.Name);
                        await _foodRepository.AddFoodAsync(food);
                    }
                    else
                    {
                        //TODO: Update primirest food identifier if we already have the food
                    }


                    foodIdsForDay.Add(food.Id);
                }

                var menuForDay = new MenuForDay(foodIdsForDay, primirestMenuForDay.Date);
                menusForDays.Add(menuForDay);
            }

            //Construct menu for week and store it
            var menuForWeek = MenuForWeek.Create(menuForWeekId, menusForDays);
            await _menuRepository.AddMenuAsync(menuForWeek);
        }

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
