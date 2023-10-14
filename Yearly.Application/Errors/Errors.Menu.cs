using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Menu
    {
        public static Error NoExternalMenusForThisWeek
            => Error.NotFound("Menu.NoExternalMenusForThisWeek", "No external menus for this week");
    }
}