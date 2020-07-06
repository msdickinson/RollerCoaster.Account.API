﻿using DickinsonBros.DateTime.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Acccount.API.Abstractions;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Logic;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Controllers;
using RollerCoaster.Account.API.View.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : BaseTest
    {
        #region CreateUserAsync

        [TestMethod]
        public async Task CreateUserAsync_Runs_AccountManagerCreateUserAsyncCalled()
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

                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, createUserAccountDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, Role.User)
                    };

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
                            new CreateUserAccountDescriptor
                            {
                                Result = CreateUserAccountResult.Successful,
                                AccountId = 1000
                            }
                        );

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
        public async Task CreateUserAsync_Successful_ReturnsTokensWithStatusCode200()
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

                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = 1000
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, createUserAccountDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, Role.User)
                    };

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
                            new CreateUserAccountDescriptor
                            {
                                Result = CreateUserAccountResult.Successful,
                                AccountId = 1000
                            }
                        );

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
                    var observed = await uut.CreateUserAccountAsync(createUserAccountRequest);

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
        public async Task CreateUserAsync_DuplicateUser_ReturnsStatusCode409()
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.CreateUserAccountAsync(createUserAccountRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAsync_GenerateTokensIsNotAuthorized_ReturnsStatusCode500()
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

                    var createUserAccountDescriptor = new CreateUserAccountDescriptor
                    {
                        Result = CreateUserAccountResult.Successful,
                        AccountId = null
                    };

                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
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
                    var observed = await uut.CreateUserAccountAsync(createUserAccountRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region CreateAdminAsync

        [TestMethod]
        public async Task CreateAdminAsync_Runs_AccountManagerCreateUserAsyncCalled()
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

                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = 1000
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, createAdminAccountDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, Role.Admin)
                    };

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
                    var observed = (await uut.CreateAdminAccountAsync(createAdminAccountRequest)) as StatusCodeResult;

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
        public async Task CreateAdminAsync_Successful_ReturnsTokensWithStatusCode200()
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

                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = 1000
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, createAdminAccountDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, Role.Admin)
                    };

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
                    var observed = await uut.CreateAdminAccountAsync(createAdminAccountRequest);

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
        public async Task CreateAdminAsync_InvaildToken_ReturnsStatusCode401()
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


                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateAdminAccountAsync(createAdminAccountRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_DuplicateUser_ReturnStatusCode409()
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

                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.DuplicateUser,
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


                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateAdminAccountAsync(createAdminAccountRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_GenerateTokensIsNotAuthorized_ReturnsStatusCode500()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAdminAccountRequest = new CreateAdminAccountRequest
                    {
                        Username = "Admin1000",
                        Password = "Password!"
                    };

                    var createAdminAccountDescriptor = new CreateAdminAccountDescriptor
                    {
                        Result = CreateAdminAccountResult.Successful,
                        AccountId = null
                    };

                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
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
                    var observed = await uut.CreateAdminAccountAsync(createAdminAccountRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region LoginAsync

        [TestMethod]
        public async Task LoginAsync_Runs_AccountManagerLoginAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "SamplePassword"
                    };

                    var role = Role.User;

                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000,
                        Role = role
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, role)
                    };

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
                    var observed = (await uut.LoginAsync(loginRequest)) as StatusCodeResult;

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
        public async Task LoginAsync__Successful_ReturnsTokensWithStatusCode200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "SamplePassword"
                    };

                    var role = Role.User;

                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000,
                        Role = role
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, role)
                    };

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
                    var observed = await uut.LoginAsync(loginRequest);

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
        public async Task LoginAsync_InvaildPassword_ReturnsStatusCode401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "SamplePassword"
                    };

                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.InvaildPassword,
                        AccountId = null,
                        Role = null
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.LoginAsync(loginRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_AccountLocked_ReturnsStatusCode403()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "SamplePassword"
                    };

                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.AccountLocked,
                        AccountId = null,
                        Role = null
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.LoginAsync(loginRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_AccountNotFound_ReturnsStatusCode404()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "SamplePassword"
                    };

                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.AccountNotFound,
                        AccountId = null,
                        Role = null
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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.LoginAsync(loginRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(404, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_GenerateTokensIsNotAuthorized_ReturnsStatusCode500()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "SampleUsername",
                        Password = "SamplePassword"
                    };

                    var role = Role.User;

                    var loginDescriptor = new LoginDescriptor
                    {
                        Result = LoginResult.Successful,
                        AccountId = 1000,
                        Role = role
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
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
                    var observed = (await uut.LoginAsync(loginRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region RefreshTokensAsync

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

                    //AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accountId)
                    };
                    var accessTokenIsAuthenticated = true;
                    var accessIdentityMock = new Mock<IIdentity>();

                    accessIdentityMock
                    .SetupGet(accessIdentity => accessIdentity.IsAuthenticated)
                    .Returns(accessTokenIsAuthenticated);



                    var accessClaimsPrincipalMock =  new Mock<ClaimsPrincipal>();

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Identity)
                    .Returns(accessIdentityMock.Object);

                    accessClaimsPrincipalMock
                    .SetupGet(accessClaimsPrincipal => accessClaimsPrincipal.Claims)
                    .Returns(accessClaims);

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
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
                    var accessAccountId = "SampleAccessAccountUsername";
                    var refreshaccountId = "SampleRefreshAccountUsername";
                    var refreshTokensRequest = new RefreshTokensRequest
                    {
                        AccessToken = "SampleAccessToken",
                        RefreshToken = "SampleRefreshToken"
                    };

                    //AcessTokenClaims
                    var accessClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, accessAccountId)
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

                    //refreshTokenClaims
                    var refreshClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, refreshaccountId)
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = new Tokens()
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = false,
                        Tokens = null
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest)) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
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

                    //AcessTokenClaims
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

                    //refreshTokenClaims
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

                    //JWT Service
                    var getPrincipalReturnsTimes = 0;
                    var getPrincipalCalledTimes = 0;

                    var getPrincipalFirstCallToken = (string)null;
                    var getPrincipalFirstCallVaildateLifetime = (bool?)null;
                    var getPrincipalFirstCallTokenType = (TokenType?)null;

                    var getPrincipalSecondCallToken = (string)null;
                    var getPrincipalSecondCallVaildateLifetime = (bool?)null;
                    var getPrincipalSecondCallTokenType = (TokenType?)null;


                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<RollerCoasterJWTServiceOptions>>();
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

                    var generateTokensClaimsObserved = (IEnumerable<Claim>)null;
                    var tokens = new Tokens();
                    var generateTokensDescriptor = new GenerateTokensDescriptor
                    {
                        Authorized = true,
                        Tokens = tokens
                    };

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

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.RefreshTokensAsync(refreshTokensRequest));

                    //Assert
                    Assert.IsInstanceOfType(observed, typeof(OkObjectResult));
                    var observedOkObjectResult = observed as OkObjectResult;

                    Assert.AreEqual(200, observedOkObjectResult.StatusCode);

                    var observedTokens = (Tokens)observedOkObjectResult.Value;
                    Assert.AreEqual(tokens, observedTokens);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }
        #endregion

        #region UpdateEmailPreferenceAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ResetPasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.Any
                    };
                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
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
                        )
                        .Returns
                        (
                             Task.CompletedTask
                        );


                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    await uut.UpdateEmailPreferenceAsync(updateEmailSettingsRequest);

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                expectedUserId,
                                updateEmailSettingsRequest.EmailPreference
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
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.Any
                    };

                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
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
                        )
                        .Returns
                        (
                             Task.CompletedTask
                        );


                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    var observed = await uut.UpdateEmailPreferenceAsync(updateEmailSettingsRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdateEmailPreferenceWithTokenAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_InvaildToken_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.Any,
                        Token = "Token"
                    };

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
                        .ReturnsAsync
                        (
                             UpdateEmailPreferenceWithTokenResult.InvaildToken
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(updateEmailSettingsRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
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
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.Any,
                        Token = "Token"
                    };

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
                        .ReturnsAsync
                        (
                             UpdateEmailPreferenceWithTokenResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(updateEmailSettingsRequest) as StatusCodeResult;

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
            serviceCollection.AddSingleton<AccountController>();
            serviceCollection.AddSingleton(Mock.Of<IAccountManager>());
            serviceCollection.AddSingleton(Mock.Of<ILoggingService<AccountController>>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<IJWTService<RollerCoasterJWTServiceOptions>>());

            return serviceCollection;
        }
        #endregion
    }
}
