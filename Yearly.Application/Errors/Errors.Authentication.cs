using ErrorOr;
using Yearly.Contracts;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials
            => Error.Validation(ErrorCodes.Authentication.InvalidCredentials, "Invalid username or password");

        public static Error CookieNotSigned
            => Error.Validation(ErrorCodes.Authentication.CookieNotSigned, "Cookie is not signed");

        public static Error SessionNotCached
            => Error.Validation(ErrorCodes.Authentication.SessionNotCached, "Session is not cached in Sharp");

        public static Error InsufficientPermissions
            => Error.Validation(ErrorCodes.Authentication.InsufficientPermissions, "Insufficient permissions");

        public static Error MissingCookie
            => Error.Validation(ErrorCodes.Authentication.MissingCookie, "Missing cookie");
    }
}