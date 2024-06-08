using ErrorOr;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Services.Menus;

/// <summary>
/// Use mocked data to be more gentle to Primirest API during development
/// </summary>
public class PrimirestMenuProviderDev : IPrimirestMenuProvider
{
    private readonly IWebHostEnvironment _env;

    public PrimirestMenuProviderDev(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<ErrorOr<List<PrimirestWeeklyMenu>>> GetMenusThisWeekAsync()
    {
        // Copied to build folder
        var menusJson = await File.ReadAllTextAsync(
            Path.Combine(
                _env.ContentRootPath,
                "..", 
                "Yearly.Infrastructure",
                "Services",
                "Menus",
                "SampleMenuMock.json"),
            Encoding.UTF8);

        var menus = JsonConvert.DeserializeObject<List<PrimirestWeeklyMenu>>(menusJson)!;

        // Shift the menus such the first menu starts this monday
        var today = DateTime.Today;
        var thisWeekMonday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        var youngestMenu = menus.MinBy(wm => wm.DailyMenus.First().Date);
        var shift =  thisWeekMonday - youngestMenu.DailyMenus.First().Date;

        // Create new menus with shift
        return menus.Select(wm => wm
            with {DailyMenus = wm.DailyMenus
                .Select(dm => dm with {Date = dm.Date + shift})
                .ToList()})
            .ToList();
    }

    public Task<int[]> GetMenuIdsAsync(HttpClient adminSessionLoggedClient)
        => Task.FromResult(new[] {141248686, 144357172, 146494128, 148185209});
}