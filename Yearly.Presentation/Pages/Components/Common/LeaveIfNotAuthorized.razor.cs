using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Authentication;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components.Common;

public partial class LeaveIfNotAuthorized : IDisposable
{
    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;

    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public UserRoleDTO[]? RequiredRoles { get; set; }
    [Parameter] public string ToLink { get; set; } = "/";


    public void Dispose()
    {
        _sessionDetailsService.OnSessionDetailsChanged -= OnSessionDetailsChanged;
    }

    protected override void OnParametersSet()
    {
        CheckAuthorized();

        _sessionDetailsService.OnSessionDetailsChanged += OnSessionDetailsChanged;
    }

    private void OnSessionDetailsChanged()
    {
        CheckAuthorized();
    }

    private void CheckAuthorized()
    {
        var authorized = _sessionDetailsService is { IsAuthenticated: true, UserDetails: not null }
                     && (RequiredRoles is null || RequiredRoles.All(_sessionDetailsService.UserDetails.Value.Roles.Contains));

        if (authorized)
            return;

        _navigationManager.NavigateTo(ToLink);
    }
}