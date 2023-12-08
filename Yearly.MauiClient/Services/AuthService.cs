using Yearly.Contracts.Authentication;

namespace Yearly.MauiClient.Services;

public class AuthService
{ 
    public string? SessionCookie { get; private set; } = null;
    public UserDetailsResponse? UserDetails { get; private set; } = null;

    /// <summary>
    /// Returns true if a session cookie is set.
    /// </summary>
    public bool IsLoggedIn => SessionCookie is not null;
    
    public event Action OnLogin = null!;
    
    private const string k_SessionCookieKey = "SessionCookie";

    /// <summary>
    /// Tries to load the session from client storage. Acts as <see cref="SetSessionAsync"/>
    /// </summary>
    /// <returns>True if session is loaded and valid, False if session is invalid.</returns>
    public async Task<bool> TryLoadSessionAsync()
    {
        var sessionCookie = await SecureStorage.GetAsync(k_SessionCookieKey);
        if (sessionCookie is null)
        {
            return false;
        }

        //Check that the session is valid
        //Todo:

        SessionCookie = sessionCookie;
        OnLogin?.Invoke();

        return true;
    }

    public async Task SetSessionAsync(LoginResponse loginResponse)
    {
        SessionCookie = loginResponse.SessionCookie;
        UserDetails = loginResponse.UserDetails;

        await SecureStorage.SetAsync(k_SessionCookieKey, SessionCookie);

        OnLogin?.Invoke();
    }

    public record OnLoginMessage;
}