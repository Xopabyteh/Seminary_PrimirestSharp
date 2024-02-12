using Microsoft.AspNetCore.Components;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages.Components.Layout;

public partial class Sidebar : IDisposable
{
    [Inject] private SessionDetailsService _sessionDetails { get; set; }

    public void Dispose()
    {
        _sessionDetails.OnSessionDetailsChanged -= OnSessionDetailsChanged;
    }

    protected override void OnInitialized()
    {
        _sessionDetails.OnSessionDetailsChanged += OnSessionDetailsChanged;
    }

    private void OnSessionDetailsChanged()
    {
        StateHasChanged();
    }
}