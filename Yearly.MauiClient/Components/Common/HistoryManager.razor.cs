using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;

namespace Yearly.MauiClient.Components.Common;

public partial class HistoryManager : IDisposable
{
    public static HistoryManager Instance => _instance!;
    private static HistoryManager? _instance;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    private IDisposable locationChangingEventDisposable = null!;

    private string? currentPage;
    private Stack<string> pagesHistoryStack = new(6);
    private bool isGoingBackInHistory = false; //So we don't get in a loop

    /// <summary>
    /// Pages that aren't allowed to go back to. You can go to them, you can leave them,
    /// but once you leave them you cannot go back.
    /// </summary>
    private readonly string[] _forbiddenBackPages = { "/", "/login" };

    protected override void OnInitialized()
    {
        if (_instance is not null)
        {
            // This runs when a new instance of this is created even tho 
            // the singleton already exists. e.x. when F5 is hit on windows.
            // Just dispose the singleton and go with the new one.
            
            // Copy history from old instance
            this.pagesHistoryStack = new Stack<string>(_instance.pagesHistoryStack);

            _instance.Dispose();
        }
        _instance = this;

        locationChangingEventDisposable = _navigationManager.RegisterLocationChangingHandler(LocationChangingHandler);
    }

    private ValueTask LocationChangingHandler(LocationChangingContext arg)
    {
        if (!isGoingBackInHistory && currentPage is not null)
        {
            pagesHistoryStack.Push(currentPage);
        }

        var newPage = _navigationManager.ToAbsoluteUri(arg.TargetLocation).AbsolutePath;
        currentPage = newPage;

        isGoingBackInHistory = false; //Reset
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        locationChangingEventDisposable.Dispose();
    }

    /// <summary>
    /// Attempts to go back in navigation history by 1
    /// unless that would put user into splash screen or smth
    /// </summary>
    /// <returns></returns>
    public void TryGoBack()
    {
        if (_instance is null)
            return; //This should hopefully never happen

        if (pagesHistoryStack.Count == 0)
            return;

        var historyPeek = pagesHistoryStack.Peek();
        if (_forbiddenBackPages.Contains(historyPeek))
        {
            //We cannot go back.
            //Check if there are any history records behind this blockade and remove them if so
            if (pagesHistoryStack.Count > 1)
            {
                pagesHistoryStack.Clear(); //Remove everything
                pagesHistoryStack.Push(historyPeek); //Add this back
            }

            return;
        }

        pagesHistoryStack.Pop();
        isGoingBackInHistory = true;
        _navigationManager.NavigateTo(historyPeek);
    }
}