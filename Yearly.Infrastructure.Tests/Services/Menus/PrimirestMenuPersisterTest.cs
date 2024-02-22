using ErrorOr;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services.Authentication;
using Yearly.Infrastructure.Services.Menus;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Tests.Services.Menus;

public class PrimirestMenuPersisterTest
{
    [Fact]
    public async Task Persister_PersistsAllAvailableMenus()
    {
        // Arrange
        var pLoggedSessionRunnerMock = new PrimirestAdminLoggedSessionRunnerFixture();
        var weeklyMenuRepositoryMock = new WeeklyMenuRepositoryFixture();
        var foodRepository = new FoodRepositoryFixture();
        var nullLogger = new NullLogger<PrimirestMenuPersister>();
        var pMenuProviderMock = new Mock<IPrimirestMenuProvider>();
        var mockedMenus = new List<PrimirestWeeklyMenu>()
        {
            new (
                new List<PrimirestDailyMenu>()
                {
                    new PrimirestDailyMenu(
                        new DateTime(1999, 1, 1),
                        new List<PrimirestFood>()
                        {
                            new ("Jídlo1", "", new(1337, 0, 0)),
                            new ("Jídlo2", "", new(1337, 0, 1)),
                            new ("Jídlo3", "", new(1337, 0, 2)),
                        },
                        new PrimirestSoup("Polévka1")),
                    new PrimirestDailyMenu(
                        new DateTime(1999, 1, 2),
                        new List<PrimirestFood>()
                        {
                            new ("Jídlo4", "", new(1337, 1, 0)),
                            new ("Jídlo5", "", new(1337, 1, 1)),
                            new ("Jídlo6", "", new(1337, 1, 2)),
                        },
                        new PrimirestSoup("Polévka1")),
                },
                1337),
            new(
                new List<PrimirestDailyMenu>()
                {
                    new PrimirestDailyMenu(
                        new DateTime(1999, 1, 1+7),
                        new List<PrimirestFood>()
                        {
                            new ("Jídlo1", "", new(1338, 0, 0)),
                            new ("Jídlo2", "", new(1338, 0, 1)),
                            new ("Jídlo3", "", new(1338, 0, 2)),
                        },
                        new PrimirestSoup("Polévka1")),
                    new PrimirestDailyMenu(
                        new DateTime(1999, 1, 2+7),
                        new List<PrimirestFood>()
                        {
                            new ("Jídlo7", "", new(1338, 1, 0)),
                            new ("Jídlo8", "", new(1338, 1, 1)),
                            new ("Jídlo9", "", new(1338, 1, 2)),
                        },
                        new PrimirestSoup("Polévka1")),
                },
                1338)
        };
        pMenuProviderMock
            .Setup(s => s.GetMenusThisWeekAsync())
            .ReturnsAsync(mockedMenus);

        var sut = new PrimirestMenuPersister(
            weeklyMenuRepositoryMock,
            nullLogger,
            foodRepository,
            pLoggedSessionRunnerMock, 
            pMenuProviderMock.Object);

        // Act
        await sut.PersistAvailableMenusAsync();

        // Assert
            //Foods
            Assert.Equal(2, foodRepository.Foods.Count(f => f.Name == "Jídlo1"));
            Assert.Equal(2, foodRepository.Foods.Count(f => f.Name == "Jídlo2"));
            Assert.Equal(2, foodRepository.Foods.Count(f => f.Name == "Jídlo3"));

            Assert.Equal(1, foodRepository.Foods.Count(f => f.Name == "Jídlo4"));
            Assert.Equal(1, foodRepository.Foods.Count(f => f.Name == "Jídlo5"));
            Assert.Equal(1, foodRepository.Foods.Count(f => f.Name == "Jídlo6"));
            Assert.Equal(1, foodRepository.Foods.Count(f => f.Name == "Jídlo7"));
            Assert.Equal(1, foodRepository.Foods.Count(f => f.Name == "Jídlo8"));
            Assert.Equal(1, foodRepository.Foods.Count(f => f.Name == "Jídlo9"));
            
            //Menus
            Assert.Equal(2, weeklyMenuRepositoryMock.WeeklyMenus.Count);
    }

    private class FoodRepositoryFixture : IFoodRepository
    {
        public List<Food> Foods { get; init; } = new();
        public Task<Food?> GetFoodByIdAsync(FoodId id)
        {
            return Task.FromResult(Foods.SingleOrDefault(f => f.Id == id));
        }

        public Task<Dictionary<int, Food>> GetFoodsByPrimirestItemIdsAsync(List<int> itemIds)
        {
            return Task.FromResult(Foods
                           .Where(f => itemIds.Contains(f.PrimirestFoodIdentifier.ItemId))
                           .ToDictionary(f => f.PrimirestFoodIdentifier.ItemId));
        }

        public Task UpdateFoodAsync(Food food)
        {
            var storedFood = Foods.SingleOrDefault(f => f.Id == food.Id);
            if (storedFood is not null)
            {
                Foods.Remove(storedFood);
                Foods.Add(food);
            }
            return Task.CompletedTask;
        }

        public Task AddFoodAsync(Food food)
        {
            Foods.Add(food);
            return Task.CompletedTask;
        }

        public Task<bool> DoesFoodWithPrimirestIdentifierExistAsync(PrimirestFoodIdentifier id)
        {
            return Task.FromResult(Foods.Any(f => f.PrimirestFoodIdentifier == id));
        }
    }

    private class WeeklyMenuRepositoryFixture : IWeeklyMenuRepository
    {
        public List<WeeklyMenu> WeeklyMenus { get; init; } = new();
        public Task AddMenuAsync(WeeklyMenu weeklyMenu)
        {
            WeeklyMenus.Add(weeklyMenu);
            return Task.CompletedTask;
        }

        public Task<bool> DoesMenuExist(WeeklyMenuId id)
        {
            return Task.FromResult(WeeklyMenus.Any(m => m.Id == id));
        }

        public Task<List<WeeklyMenuId>> GetWeeklyMenuIdsAsync()
        {
            return Task.FromResult(WeeklyMenus.Select(m => m.Id).ToList());
        }

        public Task<int> ExecuteDeleteMenusAsync(List<WeeklyMenuId> menuIds)
        {
            var count = WeeklyMenus.RemoveAll(m => menuIds.Contains(m.Id));
            return Task.FromResult(count);
        }
    }

    private class PrimirestAdminLoggedSessionRunnerFixture : IPrimirestAdminLoggedSessionRunner
    {
        public Task<ErrorOr<T>> PerformAdminLoggedSessionAsync<T>(Func<HttpClient, Task<ErrorOr<T>>> action)
        {
            return action(new HttpClient());
        }
    }
}