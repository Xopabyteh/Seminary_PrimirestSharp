namespace Yearly.Contracts;

public static partial class ErrorCodes
{
    public static class Authentication
    {
        public const string InvalidCredentials = "Authentication.InvalidCredentials";
        public const string CookieNotSigned = "Authentication.CookieNotSigned";
        public const string SessionNotCached = "Authentication.SessionNotCached";
        public const string InsufficientPermissions = "Authentication.InsufficientPermissions";
        public const string MissingCookie = "Authentication.MissingCookie";
    }
}