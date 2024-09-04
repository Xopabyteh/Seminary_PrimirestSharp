namespace Yearly.MauiClient.Services;
public class DeployEnvironmentAccessor
{
    public DeployEnvironment Env { get; init; }

    public DeployEnvironmentAccessor(DeployEnvironment env)
    {
        Env = env;
    }
}