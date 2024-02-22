using ErrorOr;

namespace Yearly.Infrastructure.Services.Authentication;

/// <summary>
/// Provides a method to run a function using an <see cref="HttpClient"/> pre-setup with
/// a session, created using the <see cref="PrimirestAdminCredentialsOptions"/>.
/// </summary>
public interface IPrimirestAdminLoggedSessionRunner
{
    public Task<ErrorOr<TResult>> PerformAdminLoggedSessionAsync<TResult>(Func<HttpClient, Task<ErrorOr<TResult>>> action);
}