using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Authentication.Queries;
using Yearly.Contracts.Authentication;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components;

public partial class App
{
    [Parameter] public string? SessionCookie { get; set; }
    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private IMapper _mapper { get; set; } = null!;

    /// <summary>
    /// Attempts to load a stored session cookie and set it as the session
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //var sessionCookie = await _browserCookieService.ReadCookieAsync(Yearly.Contracts.Authentication.SessionCookieDetails.Name);

        if (string.IsNullOrEmpty(SessionCookie))
            return;

        //Load our details
        var query = new UserBySessionQuery(SessionCookie);
        var userResult = await _mediator.Send(query);
        if (userResult.IsError)
        {
            //Cookie is invalid..
            return;
        }

        // -> Sucessfully loaded details
        //var userDetailsReadonly = _mapper.Map<UserDetailsResponse>(userDetailsResult.Value);
        _sessionDetailsService.Init(SessionCookie, userResult.Value);
    }
}