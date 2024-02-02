using Hangfire.Dashboard;
using MediatR;
using Yearly.Application.Authentication.Queries;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.BackgroundJobs;

public class PrimirestSharpAdminHangfireDashboardAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{ 
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        //Get cookie session
        var sessionCookie = context.GetHttpContext().Request.Cookies["session"];
        if (string.IsNullOrWhiteSpace(sessionCookie))
        {
            return false;
        }

        //Get session from cache
        var query = new UserBySessionQuery(sessionCookie);

        using var scope = context.GetHttpContext().RequestServices.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var userResult = await mediator.Send(query);

        if (userResult.IsError)
        {
            return false;
        }

        var user = userResult.Value;
        if (!user.Roles.Contains(UserRole.Admin))
        {
            return false;
        }

        // -> Success, user is psharp admin
        return true;
    }
}