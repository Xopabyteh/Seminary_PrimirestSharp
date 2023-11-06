using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services;

namespace Yearly.Application.Menus.Commands;

/// <summary>
/// Get's menus from primirest and persists foods and menus for those foods into our db.
/// TODO: Updates order identifiers for foods if they already exist.
/// </summary>
public class PersistAvailableMenusCommandHandler : IRequestHandler<PersistAvailableMenusCommand, ErrorOr<Unit>>
{
    private readonly IPrimirestMenuProvider _primirestMenuProvider;
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PersistAvailableMenusCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PersistAvailableMenusCommandHandler(
        IPrimirestMenuProvider primirestMenuProvider,
        IWeeklyMenuRepository weeklyMenuRepository,
        IUnitOfWork unitOfWork,
        IFoodRepository foodRepository,
        ILogger<PersistAvailableMenusCommandHandler> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _primirestMenuProvider = primirestMenuProvider;
        _weeklyMenuRepository = weeklyMenuRepository;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<Unit>> Handle(PersistAvailableMenusCommand request, CancellationToken cancellationToken)
    {
        // Delete old menus 
        // Load menu from primirest
        // Persist menu

        //Delete old menus
        var today = _dateTimeProvider.CzechNow;
        var deletedCount = await _weeklyMenuRepository.ExecuteDeleteMenusBeforeDateAsync(today);
        _logger.LogInformation("Deleted {count} old menus before the date {date}", deletedCount, today);

        var primirestMenusResult = await _primirestMenuProvider.GetMenusThisWeekAsync();
        if (primirestMenusResult.IsError)
            return primirestMenusResult.Errors;

        var primirestWeeklyMenus = primirestMenusResult.Value;
        if (primirestWeeklyMenus.Count == 0)
            return Unit.Value; //No menus available -> nothing to persist

        foreach (var primirestWeeklyMenu in primirestWeeklyMenus)
        {
            var weeklyMenuId = new WeeklyMenuId(primirestWeeklyMenu.PrimirestMenuId);

            //If we already have this menu, skip it
            if (await _weeklyMenuRepository.DoesMenuForWeekExistAsync(weeklyMenuId))
                continue;

            var dailyMenus = new List<DailyMenu>(5);

            //Handle foods from this week menu
            foreach (var primirestDailyMenu in primirestWeeklyMenu.DailyMenus)
            {
                var foodIdsForDay = new List<FoodId>(3);

                //Handle foods
                foreach (var primirestFood in primirestDailyMenu.Foods)
                {
                    var food = await _foodRepository.GetFoodByNameAsync(primirestFood.Name);

                    //If we don't have food yet, create it
                    if (food is null) 
                    {
                        food = Food.Create(primirestFood.Name, primirestFood.Allergens, primirestFood.PrimirestFoodIdentifier);
                        _logger.Log(LogLevel.Information, "New food created - {foodName}", food.Name);
                        await _foodRepository.AddFoodAsync(food);
                    }
                    else
                    {
                        food.UpdatePrimirestFoodIdentifier(primirestFood.PrimirestFoodIdentifier);
                        await _foodRepository.UpdatePrimirestFoodIdentifierAsync(food);
                    }

                    foodIdsForDay.Add(food.Id);
                }

                var menuForDay = new DailyMenu(foodIdsForDay, primirestDailyMenu.Date);
                dailyMenus.Add(menuForDay);
            }

            //Construct menu for week and store it
            var menuForWeek = WeeklyMenu.Create(weeklyMenuId, dailyMenus);
            await _weeklyMenuRepository.AddMenuAsync(menuForWeek);
        }

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
