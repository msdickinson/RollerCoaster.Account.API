using DickinsonBros.Guid.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountEmail;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Models;
using RollerCoaster.Account.API.Logic.Models;
using System;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Logic.Tests.Models
{
    [TestClass]
    public class AccountManagerTests : BaseTest
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
                    var username = "User1000";
                    var password = "Password!";
                    var email = (string)null;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };
                    var activateEmailToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var emailPreferenceToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var insertAccountResult = new Infrastructure.AccountDB.Models.InsertAccountResult
                    {
                        AccountId = 1000,
                        DuplicateUser = false
                    };

                    bool newGuidBeenCalled = false;

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.Encrypt
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            encryptResult
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (() =>
                        {
                            if (!newGuidBeenCalled)
                            {
                                newGuidBeenCalled = true;
                                return activateEmailToken;
                            }
                            else
                            {
                                return emailPreferenceToken;
                            }

                        });

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertAccountAsync(It.IsAny<InsertAccountRequest>())
                    )
                    .ReturnsAsync
                    (
                        insertAccountResult
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateAsync(username, password, email);

                    //Assert
                    passwordEncryptionServiceMock
                       .Verify(
                           passwordEncryptionService => passwordEncryptionService.Encrypt
                           (
                               password,
                               null
                           ),
                           Times.Once
                       );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_Runs_GuidServiceCalledTwice()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var email = (string)null;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };
                    var activateEmailToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var emailPreferenceToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var insertAccountResult = new Infrastructure.AccountDB.Models.InsertAccountResult
                    {
                        AccountId = 1000,
                        DuplicateUser = false
                    };

                    bool newGuidBeenCalled = false;

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.Encrypt
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            encryptResult
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (() =>
                        {
                            if (!newGuidBeenCalled)
                            {
                                newGuidBeenCalled = true;
                                return activateEmailToken;
                            }
                            else
                            {
                                return emailPreferenceToken;
                            }

                        });

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertAccountAsync(It.IsAny<InsertAccountRequest>())
                    )
                    .ReturnsAsync
                    (
                        insertAccountResult
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateAsync(username, password, email);

                    //Assert
                    guidServiceMock
                       .Verify(
                           guidService => guidService.NewGuid(),
                           Times.Exactly(2)
                       );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_Runs_AccountDBServiceInsertAccountCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var email = (string)null;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };
                    var activateEmailToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var emailPreferenceToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var insertAccountResult = new Infrastructure.AccountDB.Models.InsertAccountResult
                    {
                        AccountId = 1000,
                        DuplicateUser = false
                    };

                    bool newGuidBeenCalled = false;

                    var observedInsertAccountRequest = (InsertAccountRequest)null;

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.Encrypt
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            encryptResult
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (() =>
                        {
                            if (!newGuidBeenCalled)
                            {
                                newGuidBeenCalled = true;
                                return activateEmailToken;
                            }
                            else
                            {
                                return emailPreferenceToken;
                            }

                        });

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertAccountAsync(It.IsAny<InsertAccountRequest>())
                    )
                    .Callback((InsertAccountRequest insertAccountRequest) =>
                    {
                        observedInsertAccountRequest = insertAccountRequest;
                    })
                    .ReturnsAsync
                    (
                        insertAccountResult
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateAsync(username, password, email);

                    //Assert
                    accountDBServiceMock
                        .Verify(
                            accountDBService => accountDBService.InsertAccountAsync
                            (
                                observedInsertAccountRequest
                            ),
                            Times.Once
                        );
                    Assert.IsNotNull(observedInsertAccountRequest);
                    Assert.AreEqual(username, observedInsertAccountRequest.Username);
                    Assert.AreEqual(encryptResult.Salt, observedInsertAccountRequest.Salt);
                    Assert.AreEqual(encryptResult.Hash, observedInsertAccountRequest.PasswordHash);
                    Assert.AreEqual(activateEmailToken.ToString(), observedInsertAccountRequest.ActivateEmailToken);
                    Assert.AreEqual(emailPreferenceToken.ToString(), observedInsertAccountRequest.EmailPreferenceToken);
                    Assert.AreEqual(email, observedInsertAccountRequest.Email);
                    Assert.AreEqual(EmailPreference.Any, observedInsertAccountRequest.EmailPreference);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateUser_ReturnsResultDuplicateUser()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var email = (string)null;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };
                    var activateEmailToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var emailPreferenceToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var insertAccountResult = new Infrastructure.AccountDB.Models.InsertAccountResult
                    {
                        AccountId = 1000,
                        DuplicateUser = true
                    };

                    bool newGuidBeenCalled = false;

                    var observedInsertAccountRequest = (InsertAccountRequest)null;

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.Encrypt
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            encryptResult
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (() =>
                        {
                            if (!newGuidBeenCalled)
                            {
                                newGuidBeenCalled = true;
                                return activateEmailToken;
                            }
                            else
                            {
                                return emailPreferenceToken;
                            }

                        });

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertAccountAsync(It.IsAny<InsertAccountRequest>())
                    )
                    .Callback((InsertAccountRequest insertAccountRequest) =>
                    {
                        observedInsertAccountRequest = insertAccountRequest;
                    })
                    .ReturnsAsync
                    (
                        insertAccountResult
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateAsync(username, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(CreateAccountResult.DuplicateUser, observed.Result);
                    Assert.IsNull(observed.AccountId);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_EmailIsNotNull_SendActivateEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var email = "email";

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };
                    var activateEmailToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var emailPreferenceToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var insertAccountResult = new Infrastructure.AccountDB.Models.InsertAccountResult
                    {
                        AccountId = 1000,
                        DuplicateUser = false
                    };

                    var observedEmail = (string)null;
                    var observedUsername = (string)null;
                    var observedActivateToken = (string)null;
                    var observedUpdateEmailSettingsToken = (string)null;

                    var newGuidBeenCalled = false;

                    var observedInsertAccountRequest = (InsertAccountRequest)null;

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.Encrypt
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            encryptResult
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (() =>
                        {
                            if (!newGuidBeenCalled)
                            {
                                newGuidBeenCalled = true;
                                return activateEmailToken;
                            }
                            else
                            {
                                return emailPreferenceToken;
                            }

                        });

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertAccountAsync(It.IsAny<InsertAccountRequest>())
                    )
                    .Callback((InsertAccountRequest insertAccountRequest) =>
                    {
                        observedInsertAccountRequest = insertAccountRequest;
                    })
                    .ReturnsAsync
                    (
                        insertAccountResult
                    );

                    var accountEmailServiceMock = serviceProvider.GetMock<IAccountEmailService>();
                    accountEmailServiceMock
                    .Setup
                    (
                        accountEmailService => accountEmailService.SendActivateEmailAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                       )
                    )
                    .Callback((string email, string username, string activateToken, string updateEmailSettingsToken) =>
                    {
                        observedEmail = email;
                        observedUsername = username;
                        observedActivateToken = activateToken;
                        observedUpdateEmailSettingsToken = updateEmailSettingsToken;
                    });

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateAsync(username, password, email);

                    //Assert
                    accountEmailServiceMock
                   .Verify(
                       accountEmailService => accountEmailService.SendActivateEmailAsync
                       (
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                       ),
                       Times.Exactly(1)
                   );

                    Assert.AreEqual(email, observedEmail);
                    Assert.AreEqual(username, observedUsername);
                    Assert.AreEqual(activateEmailToken.ToString(), observedActivateToken);
                    Assert.AreEqual(emailPreferenceToken.ToString(), observedUpdateEmailSettingsToken);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_Successful_ReturnAccountIdAndSuccesful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var email = (string)null;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };
                    var activateEmailToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var emailPreferenceToken = new Guid("e92ff9b6-f8cb-4cc7-8ce8-dd6431225f8d");
                    var insertAccountResult = new Infrastructure.AccountDB.Models.InsertAccountResult
                    {
                        AccountId = 1000,
                        DuplicateUser = false
                    };

                    var newGuidBeenCalled = false;

                    var observedInsertAccountRequest = (InsertAccountRequest)null;

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.Encrypt
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            encryptResult
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (() =>
                        {
                            if (!newGuidBeenCalled)
                            {
                                newGuidBeenCalled = true;
                                return activateEmailToken;
                            }
                            else
                            {
                                return emailPreferenceToken;
                            }

                        });

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertAccountAsync(It.IsAny<InsertAccountRequest>())
                    )
                    .Callback((InsertAccountRequest insertAccountRequest) =>
                    {
                        observedInsertAccountRequest = insertAccountRequest;
                    })
                    .ReturnsAsync
                    (
                        insertAccountResult
                    );

                    var accountEmailServiceMock = serviceProvider.GetMock<IAccountEmailService>();
                    accountEmailServiceMock
                    .Setup
                    (
                        accountEmailService => accountEmailService.SendActivateEmailAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                       )
                    );

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateAsync(username, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(insertAccountResult.AccountId, observed.AccountId);
                    Assert.AreEqual(CreateAccountResult.Successful, observed.Result);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }
        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountManager, AccountManager>();
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<IAccountDBService>());
            serviceCollection.AddSingleton(Mock.Of<IPasswordEncryptionService>());
            serviceCollection.AddSingleton(Mock.Of<IAccountEmailService>());

            return serviceCollection;
        }
        #endregion
    }
}
