namespace Yearly.MauiClient.Services.SharpApi;

/// <summary>
/// Used by <see cref="Yearly.MauiClient.Components.Common.CatchInvalidWebRequests"/> to set an exception,
/// which is then read by <see cref="Yearly.MauiClient.Components.Pages.WebRequestProblem.WebRequestProblemPage"/>
/// </summary>
public class WebRequestProblemService
{
    public HttpRequestException HttpRequestException { get; set; } = null!;
}