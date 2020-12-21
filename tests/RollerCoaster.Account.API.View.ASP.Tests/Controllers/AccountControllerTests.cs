using CleanArchitecture.View.Controllers.Account;
using DickinsonBros.DateTime.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.Exceptions.InvaildRequestsException;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using RollerCoaster.Account.API.View.ASP.Exceptions;
using RollerCoaster.Account.API.View.ASP.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.ASP.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : BaseTest
    {
        #region CreateUserAccountAsync

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
                        Username = "SampleUsername",
                        Password = "Password!"
                    };


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

                    //--uut
                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as StatusCodeResult;

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
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_Runs_GenerateTokensCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };



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

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as StatusCodeResult;

                    //Assert
                    jwtServiceMock
                    .Verify
                    (
                        jwtService => jwtService.GenerateTokens
                        (
                            It.IsAny<IEnumerable<Claim>>()
                        ),
                        Times.Once
                    );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_Successful_ReturnsTokensWithStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

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

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as OkObjectResult;

                    //Assert
                    Assert.IsInstanceOfType(observed, typeof(OkObjectResult));
                    var observedOkObjectResult = observed as OkObjectResult;

                    Assert.AreEqual(200, observedOkObjectResult.StatusCode);

                    var observedResult = (Tokens)observedOkObjectResult.Value;
                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessToken, observedResult.AccessToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.AccessTokenExpiresIn, observedResult.AccessTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshToken, observedResult.RefreshToken);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.RefreshTokenExpiresIn, observedResult.RefreshTokenExpiresIn);
                    Assert.AreEqual(generateTokensDescriptor.Tokens.TokenType, observedResult.TokenType);
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
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

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
                            new InvaildRequestException(invaildRequestDatas)
                        );

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as ObjectResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual(invaildRequestDatas, observed.Value);
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
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

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

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as ObjectResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual(AccountController.INVAILD_EMAIL_FORMAT, observed.Value);
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
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

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

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as ObjectResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual(AccountController.INVAILD_EMAIL_DOMAIN, observed.Value);
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
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

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

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as ObjectResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                    Assert.AreEqual(AccountController.EMAIL, observed.Value);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAccountAsync_ThrowsDuplicateUsernameException_ReturnseUsernameWithStatusCode409()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Username = "SampleUsername",
                        Password = "Password!"
                    };

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

                    //--RollerCoasterJWTServiceOptions
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateUserAccountAsync(createUserAccountRequest)) as ObjectResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                    Assert.AreEqual(AccountController.USERNAME, observed.Value);
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
                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = uut.GenerateTokens(username);

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
                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = uut.GenerateTokens(username);

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
                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = uut.GenerateTokens(username);

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
            serviceCollection.AddSingleton<AccountController>();
            serviceCollection.AddSingleton(Mock.Of<ICreateUserAccountInteractor>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<IJWTService<RollerCoasterJWTServiceOptions>>());
            return serviceCollection;
        }
        #endregion
    }
}
