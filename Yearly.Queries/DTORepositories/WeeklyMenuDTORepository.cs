﻿using Dapper;
using Yearly.Contracts.Common;
using Yearly.Contracts.Menu;

namespace Yearly.Queries.DTORepositories;

public class WeeklyMenuDTORepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public WeeklyMenuDTORepository(ISqlConnectionFactory connectionFactory)
    {
        this._connectionFactory = connectionFactory;
    }

    public async Task<AvailableMenusResponse> GetAvailableMenus()
    {
        var sql = """
                  SELECT
                    WM.Id AS PrimirestMenuId,
                    DM.Id AS DailyMenuId,
                    DM.Date AS Date,
                    F.Id AS FoodId,
                    F.Allergens AS FoodAllergens,
                    F.Name AS FoodName,
                    F.PrimirestFoodIdentifier_DayId AS PrimirestDayId,
                    F.PrimirestFoodIdentifier_ItemId AS PrimirestItemId,
                    F.PrimirestFoodIdentifier_MenuId AS PrimirestMenuId,
                    P.Link AS Link
                  FROM
                    Domain.WeeklyMenus WM
                  JOIN
                    Domain.DailyMenus DM ON WM.Id = DM.WeeklyMenuId
                  JOIN
                    Domain.MenuFoodIds MFI ON DM.Id = MFI.DailyMenuId
                  JOIN
                    Domain.Foods F ON MFI.FoodId = F.Id
                  LEFT JOIN
                    Domain.Photos P ON (P.IsApproved = 1) AND ((P.FoodId_Value = F.Id) OR (P.FoodId_Value = F.AliasForFoodId));
                  """;

        var weeklyMenuVms = new Dictionary<int, WeeklyMenuVm>();
        var dailyMenuVms = new Dictionary<int, DailyMenuVm>();
        var foodVms = new Dictionary<Guid, FoodVm>();

        await using var connection = _connectionFactory.Create();
        await connection.QueryAsync<WeeklyMenuVm, DailyMenuVm, FoodVm, PhotoVm?, WeeklyMenuVm>(
            sql,
            (weeklyMenu, dailyMenu, food, photo) =>
            {
                if (!weeklyMenuVms.TryGetValue(weeklyMenu.PrimirestMenuId, out _))
                {
                    weeklyMenu.DailyMenus = new();
                    weeklyMenuVms.Add(weeklyMenu.PrimirestMenuId, weeklyMenu);
                }

                if (!dailyMenuVms.TryGetValue(dailyMenu.DailyMenuId, out _))
                {
                    dailyMenu.Foods = new();
                    dailyMenuVms.Add(dailyMenu.DailyMenuId, dailyMenu);
                    weeklyMenuVms[weeklyMenu.PrimirestMenuId].DailyMenus.Add(dailyMenu); //Add daily menu to weekly menu
                }

                if (!foodVms.TryGetValue(food.FoodId, out var foodEntry))
                {
                    food.PhotoLinks = new();
                    foodVms.Add(food.FoodId, food);
                    dailyMenuVms[dailyMenu.DailyMenuId].Foods.Add(food); //Add food to daily menu
                    foodEntry = food;
                }

                if (photo is not null)
                {
                    foodEntry.PhotoLinks.Add(photo.Link);
                }

                return weeklyMenu; //We don't care about this anyway lol
            }, splitOn: "PrimirestMenuId,DailyMenuId,FoodId,Link");

        //Map to List<WeeklyMenuDTO>
        var weeklyMenus = weeklyMenuVms
            .Values
            .Select(vm => new WeeklyMenuDTO(vm.DailyMenus.Select(dVm => new DailyMenuDTO(dVm.Date, dVm.Foods.Select(fVm => new FoodDTO(
                fVm.FoodName,
                fVm.FoodAllergens,
                fVm.PhotoLinks,
                fVm.FoodId,
                new(
                    fVm.PrimirestMenuId,
                    fVm.PrimirestDayId,
                    fVm.PrimirestItemId)
                )).ToList())).ToList(), vm.PrimirestMenuId)).ToList();

        return new AvailableMenusResponse(weeklyMenus);
    }

    //For simpler mapping of primirest identifier
    private sealed record FoodVm(
        Guid FoodId,
        string FoodAllergens,
        string FoodName,
        int PrimirestDayId,
        int PrimirestItemId,
        int PrimirestMenuId)
    {
        public List<string> PhotoLinks { get; set; }
    }

    private record PhotoVm(string Link);

    private record DailyMenuVm(int DailyMenuId, DateTime Date)
    {
        public List<FoodVm> Foods { get; set; }
    }

    private record WeeklyMenuVm(int PrimirestMenuId)
    {
        public List<DailyMenuVm> DailyMenus { get; set; }
    }
}