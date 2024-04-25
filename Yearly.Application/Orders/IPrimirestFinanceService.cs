using ErrorOr;
using Yearly.Domain.Models.Common.ValueObjects;

namespace Yearly.Application.Orders;

public interface IPrimirestFinanceService
{
    public Task<ErrorOr<UserFinanceDetails>> GetFinanceDetailsForUser(string sessionCookie);
}