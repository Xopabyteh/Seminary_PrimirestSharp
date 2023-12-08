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

    protected override void OnInitialized()
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
                     && AuthService.UserDetails is not null
                     && (RequiredRoles is null || RequiredRoles.All(AuthService.UserDetails.Value.Roles.Contains));
    }

    public void Dispose()
    {
        AuthService.OnLogin -= OnLogin;
    }
}