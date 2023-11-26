using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Yearly.MauiClient.Components.Pages.Auth;

public partial class AuthPage
{
    private readonly ILogger<AuthPage> _logger;

    public AuthPage(ILogger<AuthPage> logger)
    {
        _logger = logger;
    }

    [SupplyParameterFromForm]
    public string ModelUsername { get; set; }

    [SupplyParameterFromForm]
    public string ModelPassword { get; set; }

    private void SubmitLogin()
    {
        _logger.LogInformation("Login");
        _logger.LogInformation($"u: {ModelUsername}, p: {ModelPassword}");
    }
}