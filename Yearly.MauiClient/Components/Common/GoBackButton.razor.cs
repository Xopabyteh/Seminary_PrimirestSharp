using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Common;

public partial class GoBackButton
{
    [Parameter] public required string Text { get; set; }

    [Inject] private HistoryService _history { get; set; } = null!;

    public Task GoBack()
    {
        return _history.TryGoBackAsync();
    }
}