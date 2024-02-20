using ErrorOr;
using MediatR;
using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestMenuProvider
{
    public Task<ErrorOr<List<Food>>> PersistAvailableMenusAsync();
    public Task<ErrorOr<Unit>> DeleteOldMenusAsync();
}