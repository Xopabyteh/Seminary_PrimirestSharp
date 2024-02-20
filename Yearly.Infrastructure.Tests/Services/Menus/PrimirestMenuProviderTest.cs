//Not even worth typing out...

//using ErrorOr;
//using MediatR;
//using Microsoft.Extensions.Logging.Abstractions;
//using Moq;
//using System.Net;
//using Yearly.Infrastructure.Persistence.Repositories;
//using Yearly.Infrastructure.Services.Authentication;
//using Yearly.Infrastructure.Services.Menus;

//namespace Yearly.Infrastructure.Tests.Services.Menus;

//public class PrimirestMenuProviderTest
//{
//    //[Fact]
//    //public async Task Provider_PersistsMenus()
//    //{
//    //    // Arrange
//    //    var pAuthServiceMock = new Mock<PrimirestAuthService>();
//    //    var weeklyMenuRepositoryMock = new Mock<WeeklyMenuRepository>();
//    //    var foodRepository = new Mock<FoodRepository>();
//    //    var nullLogger = new NullLogger<PrimirestMenuProvider>();

//    //    var menuProvider = new PrimirestMenuProvider(
//    //        pAuthServiceMock.Object,
//    //        weeklyMenuRepositoryMock.Object,
//    //        nullLogger,
//    //        foodRepository.Object
//    //    );

//    //    // Act
//    //    await menuProvider.PersistAvailableMenusAsync();

//    //    // Assert
//    //}

//    //[Fact]
//    //public async Task Provider_GetsMenusFromRequest()
//    //{
//    //    // Arrange
//    //    string foodsJson = PrimirestMenuProviderSampleResponseJSON.SampleGetMenusForWeekResponse;

//    //    var httpClientMock = new Mock<HttpClient>();
//    //    httpClientMock
//    //        .Setup(s => s.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get)))
//    //        .Returns(Task.FromResult(new HttpResponseMessage
//    //        {
//    //            StatusCode = HttpStatusCode.OK,
//    //            Content = new StringContent(foodsJson)
//    //        }));

//    //    var pAuthServiceMock = new Mock<PrimirestAuthService>();
//    //    pAuthServiceMock //Ignore sign in, only run provided method
//    //        .Setup(s => s.PerformAdminLoggedSessionAsync<Unit>(
//    //            It.IsAny<Func<HttpClient, Task<ErrorOr<Unit>>>>()
//    //        ))
//    //        .Returns<Func<HttpClient, Task<ErrorOr<Unit>>>>(f => f(httpClientMock.Object));

//    //    var weeklyMenuRepositoryMock = new Mock<WeeklyMenuRepository>();
//    //    var foodRepository = new Mock<FoodRepository>();
//    //    var nullLogger = new NullLogger<PrimirestMenuProvider>();

//    //    var menuProvider = new PrimirestMenuProvider(
//    //        pAuthServiceMock.Object,
//    //        weeklyMenuRepositoryMock.Object,
//    //        nullLogger,
//    //        foodRepository.Object
//    //    );

//    //    // Act
//    //    var result = await menuProvider.GetMenusThisWeekAsync();
//    //}
//}