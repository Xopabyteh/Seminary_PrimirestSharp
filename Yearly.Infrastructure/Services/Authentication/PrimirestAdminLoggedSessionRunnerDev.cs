using ErrorOr;

namespace Yearly.Infrastructure.Services.Authentication;

public class PrimirestAdminLoggedSessionRunnerDev : IPrimirestAdminLoggedSessionRunner
{
    /// <summary>
    /// Ignores any logging in and out, simply run the function with a blank http client.
    /// Facilitates <b>NO</b> interaction with the actual API whatsoever
    /// </summary>
    public Task<ErrorOr<TResult>> PerformAdminLoggedSessionAsync<TResult>(
        Func<HttpClient, Task<ErrorOr<TResult>>> action)
        => action(new HttpClient());
}