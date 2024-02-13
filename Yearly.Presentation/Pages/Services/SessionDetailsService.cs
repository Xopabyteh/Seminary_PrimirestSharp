using Yearly.Contracts.Authentication;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Presentation.Pages.Services;

public class SessionDetailsService
{
    public string? SessionCookie { get; private set; }
    public User? User { get; private set; }
    public bool IsAuthenticated => SessionCookie is not null && User is not null;

    public event Action? OnSessionDetailsChanged;

    /// <summary>
    /// Sets details for this session. Doesn't write the cookie to the browser!
    /// </summary>
    public void Init(string sessionCookie, User user)
    {
        SessionCookie = sessionCookie;
        User = user;

        OnSessionDetailsChanged?.Invoke();
    }
}