using Dickinsonbros.Middleware.Function;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Acccount.API.Abstractions;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Logic;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Function.Functions;
using RollerCoaster.Account.API.View.Function.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Tests.Functions
{
    [TestClass]
    public class AccountAPITests : BaseTest
    {

        #region ActivateEmailAsync

        [TestMethod]
        public async Task ActivateEmailAsync_Runs_ActivateEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e"
                    };

                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(activateEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.Successful
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                activateEmailRequest.Token
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e"
                    };

                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(activateEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.Successful
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_InvaildPayload_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = ""
                    };

                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(activateEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.Successful
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Token\"],\"ErrorMessage\":\"The Token field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_InvaildTokenFormat_Returns400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "InvaildTokenFormat"
                    };

                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(activateEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.InvaildToken
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();
                    var uutConcrete = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_EmailWasAlreadyActivated_Returns400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e"
                    };

                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(activateEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.EmailWasAlreadyActivated
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();
                    var uutConcrete = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.EMAIL_HAS_ALREADY_BEEN_ACTIVATED, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_InvaildToken_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e"
                    };

                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(activateEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.InvaildToken
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();
                    var uutConcrete = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region CreateAdminAccountAsync
        [TestMethod]
        public async Task CreateAdminAccountAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Username\"],\"ErrorMessage\":\"The Username field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize("}"));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_Runs_AccountManagerCreateUserAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                   .Verify(
                       accountManager => accountManager.CreateAdminAsync
                       (
                           createAdminAccountRequest.Username,
                           createAdminAccountRequest.Token,
                           createAdminAccountRequest.Password,
                           createAdminAccountRequest.Email
                       ),
                       Times.Once
                   );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_Runs_ReturnsTokensWithStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);

                    var observedTokenContent = System.Text.Json.JsonSerializer.Deserialize<Tokens>(observed.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessToken, observedTokenContent.AccessToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessTokenExpiresIn, observedTokenContent.AccessTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshToken, observedTokenContent.RefreshToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshTokenExpiresIn, observedTokenContent.RefreshTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.TokenType, observedTokenContent.TokenType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_InvaildToken_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.InvaildToken,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_InvaildEmailFormat_ReturnsInvaildEmailFormatWithStatusCode400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.InvaildEmailFormat,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.INVAILD_EMAIL_FORMAT, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_InvaildEmailDomain_ReturnsInvaildEmailDomainWithStatusCode400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.InvaildEmailDomain,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.INVAILD_EMAIL_DOMAIN, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_DuplicateUser_ReturnStatusCode409()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.DuplicateUser,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAdminAccountAsync_GenerateTokensIsNotAuthorized_ReturnsStatusCode500()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "User1000",
                        Token = "MyToken",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createAdminAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAdminAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAdminAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateAdminAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region CreateUserAccountAsync
        [TestMethod]
        public async Task CreateUserAccountAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Username\"],\"ErrorMessage\":\"The Username field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize("}"));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_Runs_AccountManagerCreateUserAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                   .Verify(
                       accountManager => accountManager.CreateUserAsync
                       (
                           createUserAccountRequest.Username,
                           createUserAccountRequest.Password,
                           createUserAccountRequest.Email
                       ),
                       Times.Once
                   );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_Runs_ReturnsTokensWithStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);

                    var observedTokenContent = System.Text.Json.JsonSerializer.Deserialize<Tokens>(observed.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessToken, observedTokenContent.AccessToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessTokenExpiresIn, observedTokenContent.AccessTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshToken, observedTokenContent.RefreshToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshTokenExpiresIn, observedTokenContent.RefreshTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.TokenType, observedTokenContent.TokenType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_InvaildEmailFormat_ReturnsInvaildEmailFormatWithStatusCode400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.InvaildEmailFormat,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.INVAILD_EMAIL_FORMAT, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_InvaildEmailDomain_ReturnsInvaildEmailDomainWithStatusCode400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.InvaildEmailDomain,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.INVAILD_EMAIL_DOMAIN, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_DuplicateUser_ReturnStatusCode409()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.DuplicateUser,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_GenerateTokensIsNotAuthorized_ReturnsStatusCode500()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(createUserAccountRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateUserAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region LoginAsync

        [TestMethod]
        public async Task LoginAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Username\"],\"ErrorMessage\":\"The Username field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize("{"));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_Runs_LoginAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000,
                        Role = Role.User
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                   .Verify(
                       accountManager => accountManager.LoginAsync
                       (
                           loginRequest.Username,
                           loginRequest.Password
                       ),
                       Times.Once
                   );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_InvaildPassword_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.InvaildPassword,
                        AccountId = null,
                        Role = ""
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_AccountLocked_Returns403()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.AccountLocked,
                        AccountId = null,
                        Role = ""
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_AccountNotFound_Returns404()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.AccountNotFound,
                        AccountId = null,
                        Role = ""
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(404, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_GenerateTokensIsNotAuthorized_Returns500()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000,
                        Role = "User"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_Runs_Returns200WithTokens()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(loginRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000,
                        Role = "User"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            loginDescriptor
                        );

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens
                        {
                            AccessToken = "AccessToken123",
                            AccessTokenExpiresIn = new System.DateTime(2020, 6, 23, 0, 0, 0),
                            RefreshToken = "RefreshToken123",
                            RefreshTokenExpiresIn = new System.DateTime(2020, 6, 23, 1, 0, 0),
                            TokenType = "Bearer"
                        }
                    };

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>())
                        )
                        .Returns
                        (
                            generateTokensDescriptor
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.LoginAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);

                    var observedTokenContent = System.Text.Json.JsonSerializer.Deserialize<Tokens>(observed.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessToken, observedTokenContent.AccessToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessTokenExpiresIn, observedTokenContent.AccessTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshToken, observedTokenContent.RefreshToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshTokenExpiresIn, observedTokenContent.RefreshTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.TokenType, observedTokenContent.TokenType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region RefreshTokensAsync
        [TestMethod]
        public async Task RefreshTokensAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"AccessToken\"],\"ErrorMessage\":\"The AccessToken field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize("{"));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
    
        [TestMethod]
        public async Task RefreshTokensAsync_Runs_GetPrincipalCalledOnceForAccess()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    jwtServiceMock
                       .Verify(
                           jwtService => jwtService.GetPrincipal(

                               refreshTokensRequest.AccessToken,
                               false,
                               TokenType.Access
                           ),
                           Times.Once
                       );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_Runs_GetPrincipalCalledOnceForRefresh()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    jwtServiceMock
                       .Verify(
                           jwtService => jwtService.GetPrincipal(

                               refreshTokensRequest.RefreshToken,
                               true,
                               TokenType.Refresh
                           ),
                           Times.Once
                       );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_AccessTokenClaimsIsNull_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return null;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_RefreshTokenClaimsIsNull_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return null;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_AccessIsAuthenticatedIsFalse_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = false;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_RefreshIsAuthenticatedIsFalse_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = false;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_AccessAndRefreshNameIdentifierDontMatch_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, "AnotherId")
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_GenerateTokensAuthorizedIsFalse_ReturnsStatusCode500()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokensAsync_TokensVaild_ReturnsTokensWithStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "SampleAccountUsername";

                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(refreshTokensRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);


                    //--AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);

                    var accessClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //--refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var refreshTokenIsAuthenticated = true;
                    var refreshIdentityMock = new Mock<IIdentity>();

                    refreshIdentityMock
                    .SetupGet(refreshIdentity => refreshIdentity.IsAuthenticated)
                    .Returns(refreshTokenIsAuthenticated);

                    var refreshClaimsPrincipalMock = new Mock<ClaimsPrincipal>();

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Identity)
                    .Returns(refreshIdentityMock.Object);

                    refreshClaimsPrincipalMock
                    .SetupGet(refreshClaimsPrincipal => refreshClaimsPrincipal.Claims)
                    .Returns(refreshClaims);

                    //--IJWTService<RollerCoasterJWTServiceOptions>
                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();

                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GetPrincipal
                        (
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<TokenType>()
                        )
                    )
                    .Callback((string token, bool vaildateLifetime, TokenType tokenType) =>
                    {
                        getPrincipalCalledTimes++;

                        if (getPrincipalCalledTimes == 1)
                        {
                            getPrincipalFirstCallToken = token;
                            getPrincipalFirstCallVaildateLifetime = vaildateLifetime;
                            getPrincipalFirstCallTokenType = tokenType;
                        }

                        if (getPrincipalCalledTimes == 2)
                        {
                            getPrincipalSecondCallToken = token;
                            getPrincipalSecondCallVaildateLifetime = vaildateLifetime;
                            getPrincipalSecondCallTokenType = tokenType;
                        }
                    })
                    .Returns(() =>
                    {
                        getPrincipalReturnsTimes++;

                        if (getPrincipalReturnsTimes == 1)
                        {
                            return accessClaimsPrincipalMock.Object;
                        }

                        if (getPrincipalReturnsTimes == 2)
                        {
                            return refreshClaimsPrincipalMock.Object;
                        }

                        return null;
                    });

                    jwtServiceMock
                    .Setup
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        )
                    )
                    .Callback((IEnumerable<Claim> claims) =>
                    {
                        generateTokensClaimsObserved = claims;
                    })
                    .Returns(generateTokensDescriptor);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RefreshTokensAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);

                    var observedTokenContent = System.Text.Json.JsonSerializer.Deserialize<Tokens>(observed.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessToken, observedTokenContent.AccessToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessTokenExpiresIn, observedTokenContent.AccessTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshToken, observedTokenContent.RefreshToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshTokenExpiresIn, observedTokenContent.RefreshTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.TokenType, observedTokenContent.TokenType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdateEmailPreferenceAsync
        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "1000";
                    var updateEmailPreferenceRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes("{ \"EmailPreference\":1000 }");
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<EmailPreference>()
                            )
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"EmailPreference\"],\"ErrorMessage\":\"The field EmailPreference is invalid.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "1000";
                    var updateEmailPreferenceRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes("{");
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<EmailPreference>()
                            )
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ResetPasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedUserId = "1000";
                    var expectedUserIdInt = 1000;
                    var updateEmailPreferenceRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updateEmailPreferenceRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<EmailPreference>()
                            )
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, expectedUserId)
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                expectedUserIdInt,
                                updateEmailPreferenceRequest.EmailPreference
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "1000";
                    var updateEmailPreferenceRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updateEmailPreferenceRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<EmailPreference>()
                            )
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdateEmailPreferenceWithTokenAsync
        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceWithTokenRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly,
                        Token = ""
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updateEmailPreferenceWithTokenRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updateEmailPreferenceWithTokenResult = UpdateEmailPreferenceWithTokenResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync(updateEmailPreferenceWithTokenResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Token\"],\"ErrorMessage\":\"The Token field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceWithTokenRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly,
                        Token = ""
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes("{");
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updateEmailPreferenceWithTokenResult = UpdateEmailPreferenceWithTokenResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync(updateEmailPreferenceWithTokenResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_InvaildTokenFormat_Returns400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceWithTokenRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly,
                        Token = "SampleToken"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updateEmailPreferenceWithTokenRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updateEmailPreferenceWithTokenResult = UpdateEmailPreferenceWithTokenResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync(updateEmailPreferenceWithTokenResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_InvaildToken_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceWithTokenRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly,
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updateEmailPreferenceWithTokenRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updateEmailPreferenceWithTokenResult = UpdateEmailPreferenceWithTokenResult.InvaildToken;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync(updateEmailPreferenceWithTokenResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceWithTokenRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.AccountOnly,
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updateEmailPreferenceWithTokenRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updateEmailPreferenceWithTokenResult = UpdateEmailPreferenceWithTokenResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync(updateEmailPreferenceWithTokenResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region UpdatePasswordAsync
        [TestMethod]
        public async Task UpdatePasswordAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "1000";
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "SampleExistingPassword",
                        NewPassword = ""
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updatePasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updatePasswordResult = UpdatePasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            updatePasswordResult
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdatePasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"NewPassword\"],\"ErrorMessage\":\"The NewPassword field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "1000";
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "SampleExistingPassword",
                        NewPassword = ""
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes("{");
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updatePasswordResult = UpdatePasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            updatePasswordResult
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdatePasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_Runs_UpdatePasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var userId = 1000;
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "SampleExistingPassword",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updatePasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updatePasswordResult = UpdatePasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            updatePasswordResult
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdatePasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                    .Verify(
                        accountManager => accountManager.UpdatePasswordAsync
                        (
                            userId,
                            updatePasswordRequest.ExistingPassword,
                            updatePasswordRequest.NewPassword
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_AccountIsLocked_ReturnsStatusCode403()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var userId = 1000;
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "SampleExistingPassword",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updatePasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updatePasswordResult = UpdatePasswordResult.AccountLocked;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            updatePasswordResult
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdatePasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_AccountIsLocked_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var userId = 1000;
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "SampleExistingPassword",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updatePasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updatePasswordResult = UpdatePasswordResult.InvaildExistingPassword;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            updatePasswordResult
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdatePasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_IsSuccessful_ReturnsStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var userId = 1000;
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "SampleExistingPassword",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(updatePasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var updatePasswordResult = UpdatePasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            updatePasswordResult
                        );

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
                    var middlewareServiceMock = serviceProvider.GetMock<IMiddlewareService<RollerCoasterJWTServiceOptions>>();
                    var contextObserved = (HttpContext)null;
                    var callbackObserved = (Func<ClaimsPrincipal, Task<ContentResult>>)null;
                    var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    };

                    claimsPrincipalMock
                    .SetupGet(claimsPrincipal => claimsPrincipal.Claims)
                    .Returns(claims);


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
                    .Callback((HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, string[] roles) =>
                    {
                        if (!roles.Contains(AccountAPI.USER) && !roles.Contains(AccountAPI.USER))
                        {
                            throw (new Exception("Invaild User"));
                        }
                        contextObserved = context;
                        callbackObserved = callback;
                        var result = callbackObserved.Invoke(claimsPrincipalMock.Object).Result;

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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.UpdatePasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion


        #region ResetPasswordAsync
        [TestMethod]
        public async Task ResetPasswordAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(resetPasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var resetPasswordResult = ResetPasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(resetPasswordResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Token\"],\"ErrorMessage\":\"The Token field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes("{");
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var resetPasswordResult = ResetPasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(resetPasswordResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_Runs_ResetPasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(resetPasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var resetPasswordResult = ResetPasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(resetPasswordResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                resetPasswordRequest.Token,
                                resetPasswordRequest.NewPassword
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_InvaildTokenFormat_ReturnsStatusCode400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "InvaildTokenFormat",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(resetPasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var resetPasswordResult = ResetPasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(resetPasswordResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_InvaildToken_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(resetPasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var resetPasswordResult = ResetPasswordResult.TokenInvaild;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(resetPasswordResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_TokenVaild_ReturnsStatuscode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "368f2766-5c83-426e-89f7-684bfdc3276e",
                        NewPassword = "SampleNewPassword"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(resetPasswordRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var resetPasswordResult = ResetPasswordResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(resetPasswordResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion


        #region RequestPasswordResetEmailAsync
        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_FailedDataAnnotations_Returns400WithValidationErrors()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = ""
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(requestPasswordResetEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("application/json", observed.ContentType);
                    Assert.AreEqual("[{\"MemberNames\":[\"Email\"],\"ErrorMessage\":\"The Email field is required.\"}]", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_InvaildPayload_Return400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = ""
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes("{");
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_Runs_ResetPasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "email@emailSample.com"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(requestPasswordResetEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    accountManagerMock
                         .Verify(
                             accountManager => accountManager.RequestPasswordResetEmailAsync
                             (
                                 requestPasswordResetEmailRequest.Email
                             ),
                             Times.Once
                         );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailNotFound_Returns404()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "email@emailSample.com"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(requestPasswordResetEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.EmailNotFound;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(404, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual("", observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailNotActivated_ReturnsStatusCode403AndEmailNotActivatedMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "email@emailSample.com"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(requestPasswordResetEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.EmailNotActivated;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.EMAIL_NOT_ACTIVATED_MESSAGE, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailPreferenceSetToNone_ReturnsStatusCode403AndNoEmailSentDueToEmailPreferenceMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "email@emailSample.com"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(requestPasswordResetEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                    Assert.AreEqual("text/html", observed.ContentType);
                    Assert.AreEqual(AccountAPI.NO_EMAIL_SENT_DUE_TO_EMAIL_PERFERENCEMESSAGE, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
     
        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailActivivatedAndEmailPreferenceNotSetToNone_ReturnsStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "email@emailSample.com"
                    };

                    //--HttpRequest
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(requestPasswordResetEmailRequest));
                    var stream = new MemoryStream(byteArray);

                    Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                        .SetupGet(httpRequest => httpRequest.Body)
                        .Returns(stream);

                    //--IAccountManager
                    var requestPasswordResetEmailResult = RequestPasswordResetEmailResult.Successful;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync(requestPasswordResetEmailResult);

                    //--IMiddlewareService<RollerCoasterJWTServiceOptions>
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
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(httpRequestMock.Object) as ContentResult;

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
            serviceCollection.AddSingleton<IAccountAPI, AccountAPI>();
            serviceCollection.AddSingleton<IFunctionHelperService, FunctionHelperService>();
            serviceCollection.AddSingleton(Mock.Of<IAccountManager>());
            serviceCollection.AddSingleton(Mock.Of<IJWTService<RollerCoasterJWTServiceOptions>>());
            serviceCollection.AddSingleton(Mock.Of<IMiddlewareService<RollerCoasterJWTServiceOptions>>());
            return serviceCollection;
        }

        #endregion
    }
}
