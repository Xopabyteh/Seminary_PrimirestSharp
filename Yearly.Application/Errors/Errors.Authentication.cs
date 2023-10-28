using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static readonly Error InvalidCredentials
            = Error.Validation("Authentication.InvalidCredentials", "Invalid username or password");

        public static readonly Error CookieNotSigned
            = Error.Validation("Authentication.CookieNotSigned", "Cookie is not signed");

        public static readonly Error InsufficientPermissions
            = Error.Validation("Authentication.InsufficientPermissions", "Insufficient permissions");
    }
}