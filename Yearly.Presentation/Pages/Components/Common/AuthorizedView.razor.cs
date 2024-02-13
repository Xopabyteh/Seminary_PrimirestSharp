using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Authentication;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components.Common;

public partial class AuthorizedView : IDisposable
{
    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public UserRoleDTO[]? RequiredRoles { get; set; }

    private bool authorized = false;

    public void Dispose()
    {
        _sessionDetailsService.OnSessionDetailsChanged -= OnSessionDetailsChanged;
    }

    protected override void OnParametersSet()
    {
        ResetAuthorized();

        _sessionDetailsService.OnSessionDetailsChanged += OnSessionDetailsChanged;
    }

    private void OnSessionDetailsChanged()
    {
        ResetAuthorized();

        StateHasChanged();
    }

    private void ResetAuthorized()
    {
        authorized = _sessionDetailsService is {IsAuthenticated: true, UserDetails: not null}
                     && (RequiredRoles is null || RequiredRoles.All(_sessionDetailsService.UserDetails.Value.Roles.Contains));
    }
}