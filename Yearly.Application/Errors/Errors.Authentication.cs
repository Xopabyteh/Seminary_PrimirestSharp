using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials
            => Error.Validation("Authentication.Credentials", "Invalid username or password");

        public static Error CookieNotSigned
            => Error.Validation("Authentication.CookieNotSigned", "Cookie is not signed");
    }
}