using Microsoft.AspNetCore.Components;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components.Common;

/// <summary>
/// Can be used as "AuthenticatedView" if provided <see cref="RequiredRoles"/> is null
/// </summary>
public partial class AuthorizedView : IDisposable
{
    [Parameter] public required RenderFragment? ChildContent { get; set; }
    [Parameter] public UserRole[]? RequiredRoles { get; set; }

    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;

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
        authorized = _sessionDetailsService is {IsAuthenticated: true, User: not null}
                     && (RequiredRoles is null || RequiredRoles.All(_sessionDetailsService.User!.Roles.Contains) || _sessionDetailsService.User!.Roles.Contains(UserRole.Admin));
    }
}