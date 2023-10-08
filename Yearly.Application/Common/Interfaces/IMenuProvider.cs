using ErrorOr;
using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Application.Common.Interfaces;

public interface IMenuProvider
{
    public Task<ErrorOr<List<Menu>>> GetMenusThisWeek();
}