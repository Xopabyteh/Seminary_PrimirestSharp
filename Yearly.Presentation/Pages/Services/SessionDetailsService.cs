using Yearly.Contracts.Authentication;

namespace Yearly.Presentation.Pages.Services;

public class SessionDetailsService
{
    public string? SessionCookie { get; private set; }
    public UserDetailsResponse? UserDetails { get; private set; }
    public bool IsAuthenticated => SessionCookie is not null;

    public event Action? OnSessionDetailsChanged;

    /// <summary>
    /// Sets details for this session. Doesn't write the cookie to the browser!
    /// </summary>
    public void Init(string sessionCookie, UserDetailsResponse userDetails)
    {
        SessionCookie = sessionCookie;
        UserDetails = userDetails;

        OnSessionDetailsChanged?.Invoke();
    }
}