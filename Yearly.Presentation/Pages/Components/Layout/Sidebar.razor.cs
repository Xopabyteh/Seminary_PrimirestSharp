using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Authentication.Commands;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components.Layout;

public partial class Sidebar : IDisposable
{
    [Inject] private SessionDetailsService _sessionDetails { get; set; } = null!;
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    public void Dispose()
    {
        _sessionDetails.OnSessionDetailsChanged -= OnSessionDetailsChanged;
    }

    protected override void OnInitialized()
    {
        _sessionDetails.OnSessionDetailsChanged += OnSessionDetailsChanged;
    }

    protected async Task BeginLogout()
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

    private void OnSessionDetailsChanged()
    {
        StateHasChanged();
    }

}