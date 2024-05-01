using ErrorOr;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Orders;

public interface IPrimirestFinanceService
{
    public Task<ErrorOr<UserFinanceDetails>> GetFinanceDetailsForUser(string sessionCookie, User ofUser);
}