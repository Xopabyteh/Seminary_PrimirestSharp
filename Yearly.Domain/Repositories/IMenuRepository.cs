using Yearly.Domain.Models.MenuAgg;

namespace Yearly.Domain.Repositories;
public interface IMenuRepository
{
    public Task AddMenuAsync(Menu menu);
    public Task<bool> DoesMenuExistForDateAsync(DateTime date);
}
