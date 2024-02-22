using ErrorOr;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Services.Menus;

public interface IPrimirestMenuProvider
{
    /// <summary>
    /// Get menus displayed on the Primirest page, this week.
    /// </summary>
    public Task<ErrorOr<List<PrimirestWeeklyMenu>>> GetMenusThisWeekAsync();
    
    /// <summary>
    /// Get menu ids, displayed on the Primirest page, this week
    /// </summary>
    /// <param name="adminSessionLoggedClient">The Client provided through <see cref="IPrimirestAdminLoggedSessionRunner"/></param>
    public Task<int[]> GetMenuIdsAsync(HttpClient adminSessionLoggedClient);
}