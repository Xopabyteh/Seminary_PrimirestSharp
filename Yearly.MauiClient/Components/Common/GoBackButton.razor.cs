using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Components.Layout;

namespace Yearly.MauiClient.Components.Common;

public partial class GoBackButton
{
    [Parameter] public required string Text { get; set; }

    public void GoBack()
    {
        HistoryManager.Instance.TryGoBack();
    }
}