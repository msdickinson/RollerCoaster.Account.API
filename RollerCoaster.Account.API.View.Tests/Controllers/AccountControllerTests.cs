using DickinsonBros.Account.API.View.Controllers;
using DickinsonBros.DateTime.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Encryption.JWT.RollerCoaster.Acccount.API.Infrastructure.JWT;
using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Acccount.API.Abstractions;
using RollerCoaster.Account.API.Logic;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : BaseTest
    {
        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_Runs_AccountManagerCreateAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAccountRequest = new CreateAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var createAccountDescriptor = new CreateAccountDescriptor
                    {
                        Result = CreateAccountResult.Successful,
                        AccountId = 1000
                    };

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, createAccountDescriptor.AccountId?.ToString()),
                        new Claim(ClaimTypes.Role, "User")
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
                            accountManager => accountManager.CreateAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new CreateAccountDescriptor
                            {
                                Result = CreateAccountResult.Successful,
                                AccountId = 1000
                            }
                        );

                   var jwtServiceMock = serviceProvider.GetMock<IJWTService<WebsiteJWTServiceOptions>>();
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
                    var observed = (await uut.CreateAsync(createAccountRequest)) as StatusCodeResult;

                    //Assert
                    accountManagerMock
                       .Verify(
                           accountManager => accountManager.CreateAsync
                           (
                               createAccountRequest.Username,
                               createAccountRequest.Password,
                               createAccountRequest.Email
                           ),
                           Times.Once
                       );
               },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateUser_ReturnsConflict409()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAccountRequest = new CreateAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var createAccountDescriptor = new CreateAccountDescriptor
                    {
                        Result = CreateAccountResult.DuplicateUser,
                        AccountId = null
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAccountDescriptor
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.CreateAsync(createAccountRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_GenerateTokensIsNotAuthorized_ReturnsServer()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAccountRequest = new CreateAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var createAccountDescriptor = new CreateAccountDescriptor
                    {
                        Result = CreateAccountResult.Successful,
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
                            accountManager => accountManager.CreateAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            createAccountDescriptor
                        );

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService<WebsiteJWTServiceOptions>>();
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
                    var observed = await uut.CreateAsync(createAccountRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(500, observed.StatusCode);
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
            serviceCollection.AddSingleton(Mock.Of<IJWTService<WebsiteJWTServiceOptions>>());

            return serviceCollection;
        }
        #endregion
    }
}
