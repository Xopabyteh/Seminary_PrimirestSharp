namespace Yearly.MauiClient.Services.SharpApi;

public class ApiUrlService
{
    private readonly DeployEnvironmentAccessor _deployEnvironmentAccessor;

    public ApiUrlService(DeployEnvironmentAccessor deployEnvironmentAccessor)
    {
        _deployEnvironmentAccessor = deployEnvironmentAccessor;
    }


    public const string DevBaseAddress = "https://02ps6q5z-7217.euw.devtunnels.ms/";
    public const string ProdBaseAddress = "https://primirestsharp.azurewebsites.net/";
    public string GetBaseAddress()
        => _deployEnvironmentAccessor.Env switch
        {
            DeployEnvironment.Dev => DevBaseAddress,
            DeployEnvironment.Prod => ProdBaseAddress,
            _ => throw new ArgumentOutOfRangeException()
        };

    public string RelativeToAbsoluteUrl(string relativeUrl)
        => $"{GetBaseAddress()}{relativeUrl}";
}