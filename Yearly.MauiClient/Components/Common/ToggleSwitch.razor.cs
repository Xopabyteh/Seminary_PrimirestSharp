using Microsoft.AspNetCore.Components;

namespace Yearly.MauiClient.Components.Common;

public partial class ToggleSwitch
{
    [Parameter] public bool Checked { get; set; } = false;
    /// <summary>
    /// If true, the button will predict it's next state and validate it after
    /// callback has been executed, which means it will switch instantly, be unusable until calculated
    /// and then switch back.
    /// When false, the button will pause until callback completes and then use given state.
    /// </summary>
    [Parameter] public bool IsPredictive { get; set; } = true;
    private bool waitingForCallbackResult = false;
    private bool isDisabled => IsPredictive && waitingForCallbackResult;

    /// <summary>
    /// In: Desired state
    /// Out: What should be the result state
    /// </summary>
    [Parameter] public EventCallback<bool> OnCheckedChanged { get; set; }

    protected override bool ShouldRender()
    {
        // Don't re-render when waiting for callback
            // This re-render might be caused when parameters are passed
            // back in to the toggle through the parent.
        return !isDisabled;
    }

    private async Task ChangeChecked()
    {
        if (isDisabled)
            return;

        Checked = !Checked;
        StateHasChanged();

        waitingForCallbackResult = true;
        await OnCheckedChanged.InvokeAsync(Checked);
        waitingForCallbackResult = false;
    }
}