using Microsoft.AspNetCore.Components;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components.Common;

/// <summary>
/// Can be used as "LeaveIfNotAuthenticated" when provided <see cref="RequiredRoles"/> is null (or not provided)
/// </summary>
public partial class LeaveIfNotAuthorized : IDisposable
{
    [Parameter] public required RenderFragment? ChildContent { get; set; }
    [Parameter] public UserRole[]? RequiredRoles { get; set; }
    [Parameter] public string ToLink { get; set; } = "/";

    [Inject] private SessionDetailsService _sessionDetailsService { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

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
        var authorized = _sessionDetailsService is { IsAuthenticated: true, User: not null }
                     && (RequiredRoles is null || RequiredRoles.All(_sessionDetailsService.User.Roles.Contains));

        if (authorized)
            return;

        _navigationManager.NavigateTo(ToLink);
    }
}