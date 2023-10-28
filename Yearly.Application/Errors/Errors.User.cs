using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error UserNotFound = Error.NotFound("User.UserNotFound");
    }
}