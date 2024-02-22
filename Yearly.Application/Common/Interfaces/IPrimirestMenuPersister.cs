using ErrorOr;
using MediatR;
using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Application.Common.Interfaces;

public interface IPrimirestMenuPersister
{
    public Task<ErrorOr<List<Food>>> PersistAvailableMenusAsync();
    public Task<ErrorOr<Unit>> DeleteOldMenusAsync();
}