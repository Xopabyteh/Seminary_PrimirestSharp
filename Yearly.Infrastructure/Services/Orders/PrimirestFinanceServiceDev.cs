using ErrorOr;
using HtmlAgilityPack;
using Yearly.Application.Orders;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Infrastructure.Errors;
using Yearly.Infrastructure.Http;

namespace Yearly.Infrastructure.Services.Orders;

public class PrimirestFinanceServiceDev : IPrimirestFinanceService
{
    public Task<ErrorOr<UserFinanceDetails>> GetFinanceDetailsForUser(string sessionCookie, User ofUser)
    {
        return Task.FromResult(
            new UserFinanceDetails(
                new MoneyCzechCrowns(1337),
                new MoneyCzechCrowns(420)).ToErrorOr());
    }
}