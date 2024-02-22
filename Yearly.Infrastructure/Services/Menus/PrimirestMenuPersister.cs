using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Services.Menus;

public class PrimirestMenuPersister : IPrimirestMenuPersister
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<PrimirestMenuPersister> _logger;
    private readonly IFoodRepository _foodRepository;
    private readonly IPrimirestAdminLoggedSessionRunner _loggedSessionRunner;
    private readonly IPrimirestMenuProvider _menuProvider;

    public PrimirestMenuPersister(
        IWeeklyMenuRepository weeklyMenuRepository,
        ILogger<PrimirestMenuPersister> logger, 
        IFoodRepository foodRepository,
        IPrimirestAdminLoggedSessionRunner loggedSessionRunner,
        IPrimirestMenuProvider menuProvider)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
        _foodRepository = foodRepository;
        _loggedSessionRunner = loggedSessionRunner;
        _menuProvider = menuProvider;
    }

    public async Task<ErrorOr<List<Food>>> PersistAvailableMenusAsync()
    {
        // Load menus from primirest
        var primirestMenusResult = await _menuProvider.GetMenusThisWeekAsync();
        if (primirestMenusResult.IsError)
            return primirestMenusResult.Errors;

        // Check if there are any menus
        var primirestWeeklyMenus = primirestMenusResult.Value;
        if (primirestWeeklyMenus.Count == 0)
            return new List<Food>(); //No menus available -> nothing to persist

        // If there are new menus, persist them
        var newlyPersistedFoods = await PersistNewMenusAsync(primirestWeeklyMenus);

        return newlyPersistedFoods;
    }

    /// <summary>
    /// Delete menus from our db, that are no longer accessible through primirest
    /// </summary>
    /// <returns></returns>
    public Task<ErrorOr<Unit>> DeleteOldMenusAsync()
    {
        return _loggedSessionRunner.PerformAdminLoggedSessionAsync<Unit>(async loggedClient =>
        {
            var menusOnPrimirest = await _menuProvider.GetMenuIdsAsync(loggedClient);
            var menusInDb = await _weeklyMenuRepository.GetWeeklyMenuIdsAsync();

            var menusToDelete = menusInDb
                .Where(id => !menusOnPrimirest.Contains(id.Value))
                .ToList();

            await _weeklyMenuRepository.ExecuteDeleteMenusAsync(menusToDelete);

            return Unit.Value;
        });
    }

    /// <summary>
    /// Persists foods from primirest to sharp repositories
    /// and returns the newly persisted foods (which are essentially remapped primirest foods)
    /// </summary>
    /// <returns>Returns newly persisted foods</returns>
    private async Task<List<Food>> PersistNewMenusAsync(List<PrimirestWeeklyMenu> primirestWeeklyMenusToPersist)
    {
        var newlyPersistedWeeklyMenus = new List<WeeklyMenu>(primirestWeeklyMenusToPersist.Count);
        var newlyPersistedFoods = new List<Food>(primirestWeeklyMenusToPersist.Count * 3);

        var foodsWithAlreadyExistingIdentifiers = await _foodRepository.GetFoodsWithIdentifiersThatAlreadyExistAsync(
            primirestWeeklyMenusToPersist
                .SelectMany(w => w.DailyMenus)
                .SelectMany(d => d.Foods)
                .Select(f => f.PrimirestFoodIdentifier)
                .ToList());

        foreach (var primirestWeeklyMenu in primirestWeeklyMenusToPersist)
        {
            var weeklyMenuId = new WeeklyMenuId(primirestWeeklyMenu.PrimirestMenuId);

            //If we already have this menu, skip it
            //We only want to persist menus and foods from menus, that we don't have yet
            if (await _weeklyMenuRepository.DoesMenuExist(weeklyMenuId))
                continue;

            var dailyMenus = new List<DailyMenu>(5);

            //Handle foods from this weekly menu
            foreach (var primirestDailyMenu in primirestWeeklyMenu.DailyMenus)
            {
                var foodIdsForDay = new List<FoodId>(3);

                //Handle foods
                foreach (var primirestFood in primirestDailyMenu.Foods)
                {
                    //Don't add foods that have the same primirest food identifier
                    //It might happen that the delete old menus function miss behaves
                    // and we will get duplicate primirest foods in our db. That is bad
                    if (foodsWithAlreadyExistingIdentifiers.Contains(primirestFood.PrimirestFoodIdentifier))
                    {
                        _logger.LogError("Food with primirest identifier {identifier} already exists in our db. Skipping it.", primirestFood.PrimirestFoodIdentifier);
                        continue;
                    }

                    var food = Food.Create(
                        new FoodId(Guid.NewGuid()),
                        primirestFood.Name,
                        primirestFood.Allergens,
                        primirestFood.PrimirestFoodIdentifier);

                    _logger.Log(LogLevel.Information, "New food created - {foodName}", food.Name);
                    newlyPersistedFoods.Add(food);

                    foodIdsForDay.Add(food.Id);
                }

                var menuForDay = new DailyMenu(foodIdsForDay, primirestDailyMenu.Date);
                dailyMenus.Add(menuForDay);
            }

            //Construct menu for week and store it
            var menuForWeek = WeeklyMenu.Create(weeklyMenuId, dailyMenus);
            newlyPersistedWeeklyMenus.Add(menuForWeek);
        }

        await _foodRepository.AddFoodsAsync(newlyPersistedFoods);
        await _weeklyMenuRepository.AddMenusAsync(newlyPersistedWeeklyMenus);

        return newlyPersistedFoods;
    }
}