using DickinsonBros.Guid.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
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
        public const string ADMIN_TOKEN = "ExampleToken";

        #region CreateUserAsync

        [TestMethod]
        public async Task CreateUserAsync_Runs_EncryptCalled()
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
                    var observed = await uut.CreateUserAsync(username, password, email);

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
        public async Task CreateUserAsync_Runs_GuidServiceCalledTwice()
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
                    var observed = await uut.CreateUserAsync(username, password, email);

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
        public async Task CreateUserAsync_Runs_AccountDBServiceInsertAccountCalled()
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
                    var observed = await uut.CreateUserAsync(username, password, email);

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
                    Assert.AreEqual(Role.User, observedInsertAccountRequest.Role);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAsync_DuplicateUser_ReturnsResultDuplicateUser()
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
                    var observed = await uut.CreateUserAsync(username, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(CreateUserAccountResult.DuplicateUser, observed.Result);
                    Assert.IsNull(observed.AccountId);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAsync_EmailIsNotNull_SendActivateEmailAsyncCalled()
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
                    var observed = await uut.CreateUserAsync(username, password, email);

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
        public async Task CreateUserAsync_Successful_ReturnAccountIdAndSuccesful()
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
                    var observed = await uut.CreateUserAsync(username, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(insertAccountResult.AccountId, observed.AccountId);
                    Assert.AreEqual(CreateUserAccountResult.Successful, observed.Result);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }
        #endregion

        #region CreateAdminAsync

        [TestMethod]
        public async Task CreateAdminAsync_InvaildToken_ReturnsResultInvaildToken()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var token = "Password!";
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(CreateAdminAccountResult.InvaildToken, observed.Result);
                    Assert.IsNull(observed.AccountId);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_Runs_EncryptCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";
                    var token = ADMIN_TOKEN;
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

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
        public async Task CreateAdminAsync_Runs_GuidServiceCalledTwice()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var token = ADMIN_TOKEN;
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

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
        public async Task CreateAdminAsync_Runs_AccountDBServiceInsertAccountCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var token = ADMIN_TOKEN;
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

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
                    Assert.AreEqual(Role.Admin, observedInsertAccountRequest.Role);
                    Assert.AreEqual(EmailPreference.Any, observedInsertAccountRequest.EmailPreference);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_DuplicateUser_ReturnsResultDuplicateUser()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var token = ADMIN_TOKEN;
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(CreateAdminAccountResult.DuplicateUser, observed.Result);
                    Assert.IsNull(observed.AccountId);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_EmailIsNotNull_SendActivateEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var token = ADMIN_TOKEN;
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

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
        public async Task CreateAdminAsync_Successful_ReturnAccountIdAndSuccesful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var token = ADMIN_TOKEN;
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
                    var observed = await uut.CreateAdminAsync(username, token, password, email);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(insertAccountResult.AccountId, observed.AccountId);
                    Assert.AreEqual(CreateAdminAccountResult.Successful, observed.Result);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }
        #endregion

        #region LoginAsync

        [TestMethod]
        public async Task LoginAsync_Runs_SelectAccountByUserNameAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var encryptResult = new EncryptResult
                    {
                        Hash = "hash",
                        Salt = "salt"
                    };

                    var account = new RollerCoaster.Account.API.Abstractions.Account
                    {
                        Locked = false,
                        Salt = "SampleSalt",
                        PasswordHash = "SamplePasswordHash",
                        AccountId = 1,
                        Role = Role.User
                    };

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


                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    accountDBServiceMock
                    .Verify
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<SelectAccountByUserNameRequest>()
                        ),
                        Times.Once
                    );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_SelectAccountByUserNameAsyncReturnsNull_ReturnsAccountNotFound()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var account = (RollerCoaster.Account.API.Abstractions.Account)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(LoginResult.AccountNotFound, observed.Result);
                    Assert.IsNull(observed.AccountId);
                    Assert.IsNull(observed.Role);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_SelectAccountByUserNameAsyncReturnsAccountLocked_ReturnsAccountLocked()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var account = new RollerCoaster.Account.API.Abstractions.Account
                    {
                        Locked = true
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(LoginResult.AccountLocked, observed.Result);
                    Assert.IsNull(observed.AccountId);
                    Assert.IsNull(observed.Role);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_AccountExistAndIsNotLocked_EncryptCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var salt = "SampleSalt";
                    var hash = "SampleHash";
                    var accountId = 1;
                    var role = Role.User;

                    var encryptResult = new EncryptResult
                    {
                        Hash = hash,
                        Salt = salt
                    };

                    var account = new RollerCoaster.Account.API.Abstractions.Account
                    {
                        Salt = salt,
                        PasswordHash = hash,
                        Locked = false,
                        AccountId = accountId,
                        Role = role
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );

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


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    passwordEncryptionServiceMock
                       .Verify(
                           passwordEncryptionService => passwordEncryptionService.Encrypt
                           (
                               password,
                               account.Salt
                           ),
                           Times.Once
                       );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_AccountHashAndEncpytedHashDontMatch_InsertPasswordAttemptFailedAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var salt = "SampleSalt";
                    var hash = "SampleHash";
                    var accountId = 1;
                    var role = Role.User;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "ADifferntHash",
                        Salt = salt
                    };

                    var account = new RollerCoaster.Account.API.Abstractions.Account
                    {
                        Salt = salt,
                        PasswordHash = hash,
                        Locked = false,
                        AccountId = accountId,
                        Role = role
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertPasswordAttemptFailedAsync(It.IsAny<InsertPasswordAttemptFailedRequest>())
                    );

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


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    accountDBServiceMock
                    .Verify
                    (
                        accountDBService => accountDBService.InsertPasswordAttemptFailedAsync
                        (
                            It.IsAny<InsertPasswordAttemptFailedRequest>()
                        ),
                        Times.Once
                    );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_AccountHashAndEncpytedHashDontMatch_ReturnInvaildPassword()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var salt = "SampleSalt";
                    var hash = "SampleHash";
                    var accountId = 1;
                    var role = Role.User;

                    var encryptResult = new EncryptResult
                    {
                        Hash = "ADifferntHash",
                        Salt = salt
                    };

                    var account = new RollerCoaster.Account.API.Abstractions.Account
                    {
                        Salt = salt,
                        PasswordHash = hash,
                        Locked = false,
                        AccountId = accountId,
                        Role = role
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );

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


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(LoginResult.InvaildPassword, observed.Result);
                    Assert.IsNull(observed.AccountId);
                    Assert.IsNull(observed.Role);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LoginAsync_AccountExistNotLockedAndHashsMatch_ReturnRoleAndAccountIdAndSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "User1000";
                    var password = "Password!";

                    var salt = "SampleSalt";
                    var hash = "SampleHash";
                    var accountId = 1;
                    var role = Role.User;

                    var encryptResult = new EncryptResult
                    {
                        Hash = hash,
                        Salt = salt
                    };

                    var account = new RollerCoaster.Account.API.Abstractions.Account
                    {
                        Salt = salt,
                        PasswordHash = hash,
                        Locked = false,
                        AccountId = accountId,
                        Role = role
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByUserNameAsync(It.IsAny<SelectAccountByUserNameRequest>())
                    )
                    .ReturnsAsync
                    (
                        account
                    );

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


                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.LoginAsync(username, password);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(LoginResult.Successful, observed.Result);
                    Assert.AreEqual(accountId, observed.AccountId);
                    Assert.AreEqual(role, observed.Role);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UpdateEmailPreferenceAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_UpdateEmailPreferenceAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var emailPreference = EmailPreference.Any;
                    var observedUpdateEmailPreferenceRequest = (Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup(
                        accountDBService => accountDBService.UpdateEmailPreferenceAsync
                        (
                            It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest>()
                        )
                    )
                    .Callback((Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest updateEmailPreferenceRequest) =>
                    {
                        observedUpdateEmailPreferenceRequest = updateEmailPreferenceRequest;
                    });

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    await uut.UpdateEmailPreferenceAsync(accountId, emailPreference);

                    //Assert
                    accountDBServiceMock
                    .Verify
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceAsync
                        (
                            It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest>()
                        ),
                        Times.Once
                    );

                    Assert.IsNotNull(observedUpdateEmailPreferenceRequest);
                    Assert.AreEqual(accountId, observedUpdateEmailPreferenceRequest.AccountId);
                    Assert.AreEqual(emailPreference, observedUpdateEmailPreferenceRequest.EmailPreference);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ReturnsSuccessfuly()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var emailPreference = EmailPreference.Any;
                    var observedUpdateEmailPreferenceRequest = (Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup(
                        accountDBService => accountDBService.UpdateEmailPreferenceAsync
                        (
                            It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest>()
                        )
                    )
                    .Callback((Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest updateEmailPreferenceRequest) =>
                    {
                        observedUpdateEmailPreferenceRequest = updateEmailPreferenceRequest;
                    });

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    await uut.UpdateEmailPreferenceAsync(accountId, emailPreference);

                    //Assert
                    //If Act Throws This will fail
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UpdateEmailSettingsAsync

        [TestMethod]
        public async Task UpdateEmailSettingsAsync_Runs_CallsUpdateEmailPreferencesWithTokenAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    var emailPreference = EmailPreference.Any;

                    var updateEmailPreferenceWithTokenResult = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult
                    {
                        VaildToken = true
                    };

                    var updateEmailPreferencesWithTokenRequestObserved = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = emailPreference,
                        EmailPreferenceToken = token
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest>()
                        )
                    )
                    .Callback((Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest updateEmailPreferencesWithTokenRequest) =>
                    {
                        updateEmailPreferencesWithTokenRequestObserved = updateEmailPreferencesWithTokenRequest;
                    })
                    .ReturnsAsync
                    (
                        updateEmailPreferenceWithTokenResult
                    );

                        //Act
                        var observed = await uutConcrete.UpdateEmailPreferenceWithTokenAsync(token, emailPreference);

                        //Assert
                        accountDBServiceMock
                    .Verify(
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            updateEmailPreferencesWithTokenRequestObserved
                        ),
                        Times.Once
                    );
                    Assert.AreEqual(token, updateEmailPreferencesWithTokenRequestObserved.EmailPreferenceToken);
                    Assert.AreEqual(emailPreference, updateEmailPreferencesWithTokenRequestObserved.EmailPreference);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailSettingsAsync_InvaildToken_ReturnsInvaildToken()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    var emailPreference = EmailPreference.Any;

                    var updateEmailPreferenceWithTokenResult = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult
                    {
                        VaildToken = false
                    };

                    var updateEmailPreferencesWithTokenRequestObserved = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = emailPreference,
                        EmailPreferenceToken = token
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest>()
                        )
                    )
                    .Callback((Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest updateEmailPreferencesWithTokenRequest) =>
                    {
                        updateEmailPreferencesWithTokenRequestObserved = updateEmailPreferencesWithTokenRequest;
                    })
                    .ReturnsAsync
                    (
                        updateEmailPreferenceWithTokenResult
                    );

                    //Act
                    var observed = await uutConcrete.UpdateEmailPreferenceWithTokenAsync(token, emailPreference);

                    //Assert
                    Assert.AreEqual(Abstractions.UpdateEmailPreferenceWithTokenResult.InvaildToken, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailSettingsAsync_VaildToken_ReturnsSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    var emailPreference = EmailPreference.Any;

                    var updateEmailPreferenceWithTokenResult = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult
                    {
                        VaildToken = true
                    };

                    var updateEmailPreferencesWithTokenRequestObserved = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = emailPreference,
                        EmailPreferenceToken = token
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest>()
                        )
                    )
                    .Callback((Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest updateEmailPreferencesWithTokenRequest) =>
                    {
                        updateEmailPreferencesWithTokenRequestObserved = updateEmailPreferencesWithTokenRequest;
                    })
                    .ReturnsAsync
                    (
                        updateEmailPreferenceWithTokenResult
                    );

                    //Act
                    var observed = await uutConcrete.UpdateEmailPreferenceWithTokenAsync(token, emailPreference);

                    //Assert
                    Assert.AreEqual(Abstractions.UpdateEmailPreferenceWithTokenResult.Successful, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        //[TestMethod]
        //public async Task UpdateEmailSettingsAsync_TokenNotFound_ReturnsTokenNotFound()
        //{
        //    await RunDependencyInjectedTestAsync
        //    (
        //        async (serviceProvider) =>
        //        {
        //                //Setup
        //                var uut = serviceProvider.GetRequiredService<IAccountManager>();
        //            var uutConcrete = (AccountManager)uut;
        //            var token = "1";
        //            var emailPreference = EmailPreference.Any;
        //            var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

        //            accountDBServiceStub
        //            .Setup
        //            (
        //                accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
        //                (
        //                    It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
        //                )
        //            )
        //            .ReturnsAsync
        //            (
        //                new UpdateEmailPreferencesWithTokenDBResult
        //                {
        //                    TokenFound = false
        //                }
        //            );

        //                //Act
        //                var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

        //                //Assert
        //                Assert.AreEqual(UpdateEmailSettingsResult.TokenNotFound, observed);
        //        },
        //        serviceCollection => ConfigureServices(serviceCollection)
        //    );
        //}

        //[TestMethod]
        //public async Task UpdateEmailSettingsAsync_TokenExpired_ReturnsTokenExpired()
        //{
        //    await RunDependencyInjectedTestAsync
        //    (
        //        async (serviceProvider) =>
        //        {
        //                //Setup
        //                var uut = serviceProvider.GetRequiredService<IAccountManager>();
        //            var uutConcrete = (AccountManager)uut;
        //            var token = "1";
        //            var emailPreference = EmailPreference.Any;
        //            var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

        //            accountDBServiceStub
        //            .Setup
        //            (
        //                accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
        //                (
        //                    It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
        //                )
        //            )
        //            .ReturnsAsync
        //            (
        //                new UpdateEmailPreferencesWithTokenDBResult
        //                {
        //                    TokenFound = true,
        //                    TokenExpired = true
        //                }
        //            );

        //                //Act
        //                var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

        //                //Assert
        //                Assert.AreEqual(UpdateEmailSettingsResult.TokenExpired, observed);
        //        },
        //        serviceCollection => ConfigureServices(serviceCollection)
        //    );
        //}

        //[TestMethod]
        //public async Task UpdateEmailSettingsAsync_VaildToken_ReturnsSuccessful()
        //{
        //    await RunDependencyInjectedTestAsync
        //    (
        //        async (serviceProvider) =>
        //        {
        //                //Setup
        //                var uut = serviceProvider.GetRequiredService<IAccountManager>();
        //            var uutConcrete = (AccountManager)uut;
        //            var token = "1";
        //            var emailPreference = EmailPreference.Any;
        //            var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

        //            accountDBServiceStub
        //            .Setup
        //            (
        //                accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
        //                (
        //                    It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
        //                )
        //            )
        //            .ReturnsAsync
        //            (
        //                new UpdateEmailPreferencesWithTokenDBResult
        //                {
        //                    TokenFound = true,
        //                    TokenExpired = false
        //                }
        //            );

        //                //Act
        //                var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

        //                //Assert
        //                Assert.AreEqual(UpdateEmailSettingsResult.Successful, observed);
        //        },
        //        serviceCollection => ConfigureServices(serviceCollection)
        //    );
        //}
        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountManager, AccountManager>();
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<IAccountDBService>());
            serviceCollection.AddSingleton(Mock.Of<IPasswordEncryptionService>());
            serviceCollection.AddSingleton(Mock.Of<IAccountEmailService>());

            var adminOptions = new AdminOptions
            {
                Token = ADMIN_TOKEN
            };
            var options = Options.Create(adminOptions);
            serviceCollection.AddSingleton<IOptions<AdminOptions>>(options);
            return serviceCollection;
        }
        #endregion
    }
}
