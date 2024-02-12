using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Authentication.Commands;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages;

public partial class LogoutPage
{
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private SessionDetailsService _sessionDetails { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        if (!_sessionDetails.IsAuthenticated)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }

        var command = new LogoutCommand(_sessionDetails.SessionCookie!);
        await _mediator.Send(command);

        _navigationManager.NavigateTo("/login");
    }
}