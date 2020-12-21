using Dickinsonbros.Middleware.Function;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.Exceptions.InvaildRequestsException;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using RollerCoaster.Account.API.View.Function.Exceptions;
using RollerCoaster.Account.API.View.Function.Functions.Account;
using RollerCoaster.Account.API.View.Function.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Tests.Functions.Account
{
    [TestClass]
    public class AccountAPITests : BaseTest
    {
        public class BadAccountRequest
        {
            public int Username { get; set; }
            public int Password { get; set; }
            public int Email { get; set; }
        }

        #region CreateUserAccountAsync
      
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createUserAccountRequest.Username
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
        public async Task CreateUserAccountAsync_Runs_CreateUserAccountAsyncCalled()
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    var createUserAccountRequestObserved = (CreateUserAccountRequest)null;
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .Callback((CreateUserAccountRequest createUserAccountRequestCB) =>
                        {
                            createUserAccountRequestObserved = createUserAccountRequestCB;
                        })
                        .ReturnsAsync
                        (
                            createUserAccountRequest.Username
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
                    createUserAccountInteractorMock
                    .Verify
                    (
                        createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                        (
                            It.IsAny<CreateUserAccountRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(createUserAccountRequest.Email, createUserAccountRequestObserved.Email);
                    Assert.AreEqual(createUserAccountRequest.Password, createUserAccountRequestObserved.Password);
                    Assert.AreEqual(createUserAccountRequest.Username, createUserAccountRequestObserved.Username);
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    var createUserAccountRequestObserved = (CreateUserAccountRequest)null;
                    createUserAccountInteractorMock
                    .Setup
                    (
                        createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                        (
                            It.IsAny<CreateUserAccountRequest>()
                        )
                    )
                    .Callback((CreateUserAccountRequest createUserAccountRequestCB) =>
                    {
                        createUserAccountRequestObserved = createUserAccountRequestCB;
                    })
                    .ReturnsAsync
                    (
                        createUserAccountRequest.Username
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
        public async Task CreateUserAccountAsync_ThrowsInvaildRequestException_Returns400WithInvaildRequestDatas()
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

                    //--ICreateUserAccountInteractor
                    var invaildRequestDatas = new List<InvaildRequestData>
                    {
                        new InvaildRequestData
                        {
                            PropertyName = "Username",
                            VaildationError = "Over 100 Char"
                        }
                    };
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    var createUserAccountRequestObserved = (CreateUserAccountRequest)null;
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .Callback((CreateUserAccountRequest createUserAccountRequestCB) =>
                        {
                            createUserAccountRequestObserved = createUserAccountRequestCB;
                        })
                        .ThrowsAsync
                        (
                            new InvaildRequestException(invaildRequestDatas)
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
                    Assert.AreEqual(System.Text.Json.JsonSerializer.Serialize(invaildRequestDatas), observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_ThrowsInvaildEmailFormatException_ReturnsInvaildEmailFormatWithStatusCode400()
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .ThrowsAsync
                        (
                            new InvaildEmailFormatException()
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
        public async Task CreateUserAccountAsync_ThrowsInvaildEmailDomainException_ReturnsInvaildEmailDomainWithStatusCode400()
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .ThrowsAsync
                        (
                            new InvaildEmailDomainException()
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
        public async Task CreateUserAccountAsync_ThrowsDuplicateEmailException_ReturnsEmailWithStatusCode409()
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .ThrowsAsync
                        (
                            new DuplicateEmailException()
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
                    Assert.AreEqual(AccountAPI.EMAIL, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_ThrowsDuplicateUsernameException_ReturnUsernameWithStatusCode409()
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

                    //--ICreateUserAccountInteractor
                    var createUserAccountInteractorMock = serviceProvider.GetMock<ICreateUserAccountInteractor>();
                    createUserAccountInteractorMock
                        .Setup
                        (
                            createUserAccountInteractor => createUserAccountInteractor.CreateUserAccountAsync
                            (
                                It.IsAny<CreateUserAccountRequest>()
                            )
                        )
                        .ThrowsAsync
                        (
                            new DuplicateUsernameException()
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
                    Assert.AreEqual(AccountAPI.USERNAME, observed.Content);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        #endregion

        #region GenerateTokens

        [TestMethod]
        public async Task GenerateTokens_Runs_GenerateTokensCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

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

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();
                    var uutConcrete = (AccountAPI)uut;

                    //Act
                    var observed = uutConcrete.GenerateTokens(username);

                    //Assert
                    jwtServiceMock
                    .Verify
                    (

                        jwtService => jwtService.GenerateTokens(It.IsAny<IEnumerable<Claim>>()),
                        Times.Once
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        [ExpectedException(typeof(GenerateTokensServerUnauthorized))]
        public async Task GenerateTokens_AuthorizedIsFalse_ThrowsGenerateTokensServerUnauthorized()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

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

                    //--uut                    
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();
                    var uutConcrete = (AccountAPI)uut;

                    //Act
                    var observed = uutConcrete.GenerateTokens(username);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task GenerateTokens_Runs_ReturnsTokens()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

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

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAccountAPI>();
                    var uutConcrete = (AccountAPI)uut;

                    //Act
                    var observed = uutConcrete.GenerateTokens(username);

                    //Assert
                    Assert.AreEqual(generateTokensDescriptor.Tokens, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
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
            serviceCollection.AddSingleton(Mock.Of<ICreateUserAccountInteractor>());
            serviceCollection.AddSingleton(Mock.Of<IJWTService<RollerCoasterJWTServiceOptions>>());
            serviceCollection.AddSingleton(Mock.Of<IMiddlewareService<RollerCoasterJWTServiceOptions>>());

            return serviceCollection;
        }

        #endregion

    }
}
