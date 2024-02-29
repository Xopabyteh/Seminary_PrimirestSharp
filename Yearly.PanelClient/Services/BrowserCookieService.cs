using Microsoft.JSInterop;

namespace Yearly.Presentation.Pages.Services;

public class BrowserCookieService
{
    private readonly IJSRuntime _js;

    public BrowserCookieService(IJSRuntime js)
    {
        _js = js;
    }

    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="expirationDate">Not a time offset (30m), but the date time (<see cref="DateTime.Now"/>.AddMinutes(30))</param>
    public ValueTask WriteCookieAsync(string name, string value, DateTime expirationDate)
    {
        return _js.InvokeVoidAsync("BrowserCookieService.WriteCookie", name, value, expirationDate);
    }

    /// <param name="name"></param>
    /// <returns>The value of the cookie. String may be empty, not null</returns>
    public ValueTask<string> ReadCookieAsync(string name)
    {
        return _js.InvokeAsync<string>("BrowserCookieService.ReadCookie", name);
    }

    public ValueTask DeleteCookieAsync(string name)
    {
        return _js.InvokeVoidAsync("BrowserCookieService.DeleteCookie", name);
    }
}