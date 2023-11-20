using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
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
/// </summary>
public class PersistAvailableMenusCommandHandler : IRequestHandler<PersistAvailableMenusCommand, ErrorOr<Unit>>
{
    private readonly IPrimirestMenuProvider _primirestMenuProvider;
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PersistAvailableMenusCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFoodSimilarityService _foodSimilarityService;

    public PersistAvailableMenusCommandHandler(
        IPrimirestMenuProvider primirestMenuProvider,
        IWeeklyMenuRepository weeklyMenuRepository,
        IUnitOfWork unitOfWork,
        IFoodRepository foodRepository,
        ILogger<PersistAvailableMenusCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFoodSimilarityService foodSimilarityService)
    {
        _primirestMenuProvider = primirestMenuProvider;
        _weeklyMenuRepository = weeklyMenuRepository;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _foodSimilarityService = foodSimilarityService;
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

        //Load menu from primirest
        var primirestMenusResult = await _primirestMenuProvider.GetMenusThisWeekAsync();
        if (primirestMenusResult.IsError)
            return primirestMenusResult.Errors;

        //Check if there are any
        var primirestWeeklyMenus = primirestMenusResult.Value;
        if (primirestWeeklyMenus.Count == 0)
            return Unit.Value; //No menus available -> nothing to persist

        //Persist menus
        var newlyPersistedFoods = await PersistFoodsAndMenus(primirestWeeklyMenus);

        //Create similarity table
        if (newlyPersistedFoods.Count > 0)
        {
            await _foodSimilarityService.AddToSimilarityTableAsync(newlyPersistedFoods);
        }

        await _unitOfWork.SaveChangesAsync();


        return Unit.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="primirestWeeklyMenus"></param>
    /// <returns>Newly persisted foods: foods that were not yet present in the repository and have a unique name.</returns>
    private async Task<List<Food>> PersistFoodsAndMenus(List<PrimirestWeeklyMenu> primirestWeeklyMenus)
    {
        var newlyPersistedFoods = new List<Food>(primirestWeeklyMenus.Count * 3);
        foreach (var primirestWeeklyMenu in primirestWeeklyMenus)
        {
            var weeklyMenuId = new WeeklyMenuId(primirestWeeklyMenu.PrimirestMenuId);

            //If we already have this menu, skip it
            if (await _weeklyMenuRepository.DoesMenuExist(weeklyMenuId))
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
                        food = Food.Create(
                            primirestFood.Name,
                            primirestFood.Allergens,
                            primirestFood.PrimirestFoodIdentifier);

                        _logger.Log(LogLevel.Information, "New food created - {foodName}", food.Name);
                        newlyPersistedFoods.Add(food);
                        await _foodRepository.AddFoodAsync(food);
                    }
                    else
                    {
                        food.UpdatePrimirestFoodIdentifier(primirestFood.PrimirestFoodIdentifier);
                        await _foodRepository.UpdateFoodAsync(food);
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

        return newlyPersistedFoods;
    }
}
