using ErrorOr;
using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestMenuProvider
{
    public Task<ErrorOr<List<Food>>> PersistAvailableMenusAsync();
}