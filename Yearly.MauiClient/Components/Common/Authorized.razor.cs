using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Common;

public partial class Authorized : IDisposable
{
    [Inject]
    private AuthService AuthService { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public UserRoleDTO[]? RequiredRoles { get; set; }

    private bool authorized = false;

    protected override void OnParametersSet()
    {
        ResetAuthorized();

        AuthService.OnLogin += OnLogin;
    }

    private void OnLogin()
    {
        ResetAuthorized();

        StateHasChanged();
    }

    private void ResetAuthorized()
    {
        authorized = AuthService.IsLoggedIn
                     && AuthService.ActiveUserDetails is not null
                     && (RequiredRoles is null || RequiredRoles.All(AuthService.ActiveUserDetailsLazy.Roles.Contains) || AuthService.ActiveUserDetailsLazy.Roles.Contains(UserRoleDTO.Admin));
    }

    public void Dispose()
    {
        AuthService.OnLogin -= OnLogin;
    }
}