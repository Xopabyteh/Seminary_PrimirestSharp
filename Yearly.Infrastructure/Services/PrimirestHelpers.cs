using Yearly.Contracts.Authentication;

namespace Yearly.Infrastructure.Services;

internal static class PrimirestHelpers
{
    /// <summary>
    /// Determines whether the response tried to route you to the login page,
    /// meaning you don't have a signed cookie
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static bool GotRoutedToLogin(this HttpResponseMessage response)
        => response.RequestMessage?.RequestUri?.AbsolutePath == "/CS/auth/login";
}