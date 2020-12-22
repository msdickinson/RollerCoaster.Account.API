using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.View.ASP.Controllers.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.ASP.Tests.Controllers
{
    [TestClass]
    public class StatusControllerTests : BaseTest
    {
        #region LogAsync

        [TestMethod]
        public async Task LogAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedMessage = $"{nameof(StatusController)} Test Log";
                    var loggingServiceMock = serviceProvider.GetMock<ILoggingService<StatusController>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        );

                    var uut = serviceProvider.GetControllerInstance<StatusController>();

                    //Act
                    await uut.LogAsync();

                    //Assert
                    loggingServiceMock
                       .Verify(
                            loggingService => loggingService.LogInformationRedacted
                            (
                                expectedMessage,
                                null
                            ),
                            Times.Once
                       );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LogAsync_Runs_ReturnStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedMessage = $"{nameof(StatusController)} Test Log";
                    var loggingServiceMock = serviceProvider.GetMock<ILoggingService<StatusController>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        );

                    var uut = serviceProvider.GetControllerInstance<StatusController>();

                    //Act
                    var observed = (await uut.LogAsync()) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UserAuthorizedAsync

        [TestMethod]
        public async Task UserAuthorizedAsync_Runs_ReturnStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedMessage = $"{nameof(StatusController)} Test Log";
                    var loggingServiceMock = serviceProvider.GetMock<ILoggingService<StatusController>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        );

                    var uut = serviceProvider.GetControllerInstance<StatusController>();

                    //Act
                    var observed = (await uut.UserAuthorizedAsync()) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region AdminAuthorizedAsync

        [TestMethod]
        public async Task AdminAuthorizedAsync_Runs_ReturnStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedMessage = $"{nameof(StatusController)} Test Log";
                    var loggingServiceMock = serviceProvider.GetMock<ILoggingService<StatusController>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        );

                    var uut = serviceProvider.GetControllerInstance<StatusController>();

                    //Act
                    var observed = (await uut.AdminAuthorizedAsync()) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<StatusController>();
            serviceCollection.AddSingleton(Mock.Of<ILoggingService<StatusController>>());

            return serviceCollection;
        }
        #endregion
    }
}