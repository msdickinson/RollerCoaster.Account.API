using Dickinsonbros.Middleware.Function;
using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.View.Function.Functions;
using RollerCoaster.Account.API.View.Function.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Tests.Functions.Status
{
    [TestClass]
    public class StatusAPITests : BaseTest
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
                    var expectedMessage = $"{nameof(StatusAPI)} Test Log";

                    //--HttpRequest
                    var httpRequestMock = new Mock<HttpRequest>();

                    //--ILoggingService<StatusAPI>
                    var loggingServiceMock = serviceProvider.GetMock<ILoggingService<StatusAPI>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        );

                    var uut = serviceProvider.GetRequiredService<IStatusAPI>();

                    //Act
                    await uut.LogAsync(httpRequestMock.Object);

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
                    var expectedMessage = $"{nameof(StatusAPI)} Test Log";

                    //--HttpRequest
                    var httpRequestMock = new Mock<HttpRequest>();

                    //--ILoggingService<StatusAPI>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<Task<ContentResult>>)null;
                    var contentResultObserved = new ContentResult
                    {
                    };

                    middlewareServiceMock
                    .Setup
                    (
                        middlewareService => middlewareService.InvokeAsync
                        (
                            It.IsAny<HttpContext>(),
                            It.IsAny<Func<Task<ContentResult>>>()
                        )
                    )
                    .Callback((HttpContext context, Func<Task<ContentResult>> callback) =>
                    {
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke().Result;

                        if (result == null)
                        {
                            contentResultObserved = null;
                        }
                        else
                        {
                            contentResultObserved.Content = result.Content;
                            contentResultObserved.ContentType = result.ContentType;
                            contentResultObserved.StatusCode = result.StatusCode;
                        }
                        contentResultObserved = result;
                    })
                    .ReturnsAsync(contentResultObserved);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IStatusAPI>();

                    //Act
                    var observed = await uut.LogAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);

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

                    //--HttpRequest
                    var httpRequestMock = new Mock<HttpRequest>();

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var contentResultObserved = new ContentResult
                    {
                    };

                    middlewareServiceMock
                    .Setup
                    (
                        middlewareService => middlewareService.InvokeWithJWTAuthAsync
                        (
                            It.IsAny<HttpContext>(),
                            It.IsAny<Func<ClaimsPrincipal, Task<ContentResult>>>(),
                            It.IsAny<string[]>()
                        )
                    )
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] prams) =>
                    {
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(null).Result;

                        if (result == null)
                        {
                            contentResultObserved = null;
                        }
                        else
                        {
                            contentResultObserved.Content = result.Content;
                            contentResultObserved.ContentType = result.ContentType;
                            contentResultObserved.StatusCode = result.StatusCode;
                        }
                        contentResultObserved = result;
                    })
                    .ReturnsAsync(contentResultObserved);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IStatusAPI>();

                    //Act
                    var observed = await uut.UserAuthorizedAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UserAuthorizedAsync

        [TestMethod]
        public async Task AdminAuthorizedAsync_Runs_ReturnStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--HttpRequest
                    var httpRequestMock = new Mock<HttpRequest>();

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var contentResultObserved = new ContentResult
                    {
                    };

                    middlewareServiceMock
                    .Setup
                    (
                        middlewareService => middlewareService.InvokeWithJWTAuthAsync
                        (
                            It.IsAny<HttpContext>(),
                            It.IsAny<Func<ClaimsPrincipal, Task<ContentResult>>>(),
                            It.IsAny<string[]>()
                        )
                    )
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] prams) =>
                    {
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(null).Result;

                        if (result == null)
                        {
                            contentResultObserved = null;
                        }
                        else
                        {
                            contentResultObserved.Content = result.Content;
                            contentResultObserved.ContentType = result.ContentType;
                            contentResultObserved.StatusCode = result.StatusCode;
                        }
                        contentResultObserved = result;
                    })
                    .ReturnsAsync(contentResultObserved);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IStatusAPI>();

                    //Act
                    var observed = await uut.AdminAuthorizedAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IStatusAPI, StatusAPI>();
            serviceCollection.AddSingleton<IFunctionHelperService, FunctionHelperService>();
            serviceCollection.AddSingleton(Mock.Of<ILoggingService<StatusAPI>>());
            serviceCollection.AddSingleton(Mock.Of<IMiddlewareService<RollerCoasterJWTServiceOptions>>());
            return serviceCollection;
        }

        #endregion
    }
}
