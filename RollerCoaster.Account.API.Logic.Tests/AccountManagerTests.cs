using DickinsonBros.Email.Abstractions;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Logic.Tests
{
    [TestClass]
    public class AccountManagerTests : BaseTest
    {
        public const string ADMIN_TOKEN = "ExampleToken";

        #region CreateUserAsync

        [TestMethod]
        public async Task CreateUserAsync_Runs_IsVaildEmailFormatCalled()
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateUserAsync(username, password, email);

                    //Assert
                    emailServiceMock
                        .Verify
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                email
                            ),
                            Times.Once
                        );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAsync_InvaildEmailFormat_ReturnsInvaildEmailFormat()
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

                    //--IEmailService
                    var isValidEmailFormat = false;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateUserAsync(username, password, email);

                    //Assert
                    Assert.IsNull(observed.AccountId);
                    Assert.AreEqual(CreateUserAccountResult.InvaildEmailFormat , observed.Result);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAsync_Runs_ValidateEmailDomainCalled()
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateUserAsync(username, password, email);

                    //Assert                    
                    emailServiceMock
                    .Verify
                    (
                        emailService => emailService.ValidateEmailDomain
                        (
                            email
                        ),
                        Times.Once
                    );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateUserAsync_InvaildEmailDomain_ReturnsInvaildEmailDomain()
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = false;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;

                    //Act
                    var observed = await uut.CreateUserAsync(username, password, email);

                    //Assert
                    Assert.IsNull(observed.AccountId);
                    Assert.AreEqual(CreateUserAccountResult.InvaildEmailDomain, observed.Result);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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


                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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


                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    //--IPasswordEncryptionService
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

                    //--IGuidService
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
        public async Task CreateAdminAsync_Runs_IsVaildEmailFormatCalled()
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


                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    //--IPasswordEncryptionService
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

                    //--IGuidService
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
                    emailServiceMock
                        .Verify
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                email
                            ),
                            Times.Once
                        );

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_InvaildEmailFormat_ReturnsInvaildEmailFormat()
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


                    //--IEmailService
                    var isValidEmailFormat = false;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    //--IPasswordEncryptionService
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

                    //--IGuidService
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
                    Assert.IsNull(observed.AccountId);
                    Assert.AreEqual(CreateAdminAccountResult.InvaildEmailFormat, observed.Result);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_Runs_ValidateEmailDomainCalled()
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


                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    //--IPasswordEncryptionService
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

                    //--IGuidService
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
                    emailServiceMock
                    .Verify
                    (
                        emailService => emailService.ValidateEmailDomain
                        (
                            email
                        ),
                        Times.Once
                    );
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAdminAsync_InvaildEmailDomain_ReturnsInvaildEmailDomain()
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


                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = false;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

                    //--IPasswordEncryptionService
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

                    //--IGuidService
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
                    Assert.IsNull(observed.AccountId);
                    Assert.AreEqual(CreateAdminAccountResult.InvaildEmailDomain, observed.Result);

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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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
                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
                        );

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

                    //--IEmailService
                    var isValidEmailFormat = true;
                    var validateEmailDomain = true;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.IsValidEmailFormat
                            (
                                It.IsAny<string>()
                            )
                        )
                        .Returns
                        (
                            isValidEmailFormat
                        );

                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.ValidateEmailDomain
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            validateEmailDomain
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
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
                           username
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        account
                    );

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertPasswordAttemptFailedAsync
                        (
                            It.IsAny<int>()
                        )
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
                            accountId
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
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
                        accountDBService => accountDBService.SelectAccountByUserNameAsync
                        (
                            It.IsAny<string>()
                        )
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

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup(
                        accountDBService => accountDBService.UpdateEmailPreferenceAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<EmailPreference>()
                        )
                    );

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
                           accountId,
                           emailPreference
                        ),
                        Times.Once
                    );
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

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup(
                        accountDBService => accountDBService.UpdateEmailPreferenceAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<EmailPreference>()
                        )
                    );

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
                            It.IsAny<string>(),
                            It.IsAny<EmailPreference>()
                        )
                    )
                    .ReturnsAsync
                    (
                        updateEmailPreferenceWithTokenResult
                    );

                    //Act
                    var observed = await uutConcrete.UpdateEmailPreferenceWithTokenAsync(token, emailPreference);

                    //Assert
                    accountDBServiceMock.
                    Verify
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            token,
                            emailPreference
                        ),
                        Times.Once
                    );
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

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<EmailPreference>()
                        )
                    )
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

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdateEmailPreferenceWithTokenAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<EmailPreference>()
                        )
                    )
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

        #endregion

        #region ActivateEmailAsync

        [TestMethod]
        public async Task ActivateEmailAsync_Runs_CallsActivateEmailWithTokenAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "1";

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                       new ActivateEmailWithTokenResult
                       {
                           EmailWasAlreadyActivated = false,
                           VaildToken = true
                       }
                    );

                    //Act
                    var observed = await uutConcrete.ActivateEmailAsync(token);

                    //Assert
                    accountDBServiceMock
                    .Verify(
                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
                        (
                            token
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_InvaildToken_ReturnsInvaildToken()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "1";

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                       new ActivateEmailWithTokenResult
                       {
                           EmailWasAlreadyActivated = false,
                           VaildToken = false
                       }
                    );

                    //Act
                    var observed = await uutConcrete.ActivateEmailAsync(token);

                    //Assert
                    Assert.AreEqual(ActivateEmailResult.InvaildToken, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_EmailWasAlreadyActivated_ReturnsEmailWasAlreadyActivated()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "1";

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                       new ActivateEmailWithTokenResult
                       {
                           EmailWasAlreadyActivated = true,
                           VaildToken = true
                       }
                    );

                    //Act
                    var observed = await uutConcrete.ActivateEmailAsync(token);

                    //Assert
                    Assert.AreEqual(ActivateEmailResult.EmailWasAlreadyActivated, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_VaildTokenAndNotAlreadyActivated_ReturnsSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "1";
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                       new ActivateEmailWithTokenResult
                       {
                           EmailWasAlreadyActivated = false,
                           VaildToken = true
                       }
                    );

                    //Act
                    var observed = await uutConcrete.ActivateEmailAsync(token);

                    //Assert
                    Assert.AreEqual(ActivateEmailResult.Successful, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdatePasswordAsync

        [TestMethod]
        public async Task UpdatePasswordAsync_Runs_CallsSelectAccountByAccountIdAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
                        )
                        .ReturnsAsync
                        (
                            (Abstractions.Account)null
                        );

                        //Act
                        var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                        //Assert
                        accountDBServiceMock
                        .Verify(
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                accountId
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_AccountNotFound_ReturnsAccountNotFound()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
                        )
                        .ReturnsAsync
                        (
                            (Abstractions.Account)null
                        );

                        //Act
                        var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                        //Assert
                        Assert.AreEqual(UpdatePasswordResult.AccountNotFound, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_AccountLocked_ReturnsAccountNotFound()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new Abstractions.Account
                            {
                                Locked = true
                            }
                        );

                        //Act
                        var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                        //Assert
                        Assert.AreEqual(UpdatePasswordResult.AccountLocked, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_AccountSelected_EncryptExistingPassword()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                        //Setup
                        var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var account = new Abstractions.Account
                    {
                        AccountId = accountId,
                        PasswordHash = "SampleHashOne",
                        Salt = "SampleSaltOne"
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
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
                            new EncryptResult
                            {
                                Hash = "SampleHashTwo",
                                Salt = "SampleSaltTwo"
                            }
                        );

                    //Act
                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                    //Assert
                    passwordEncryptionServiceMock
                    .Verify
                    (
                        passwordEncryptionService => passwordEncryptionService.Encrypt
                        (
                            existingPassword,
                            account.Salt
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_InvaildExistingPassword_ReturnsInvaildExistingPassword()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var account = new Abstractions.Account
                    {
                        AccountId = accountId,
                        PasswordHash = "SampleHashOne",
                        Salt = "SampleSaltOne"
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
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
                            new EncryptResult
                            {
                                Hash = "SampleHashTwo",
                                Salt = "SampleSaltTwo"
                            }
                        );

                        //Act
                        var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                        //Assert
                        Assert.AreEqual(UpdatePasswordResult.InvaildExistingPassword, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_ExistingPasswordsMatch_EncryptNewPassword()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var account = new Abstractions.Account
                    {
                        AccountId = accountId,
                        PasswordHash = "SampleHashOne",
                        Salt = "SampleSaltOne"
                    };

                    var encryptResultsIndex = 0;
                    var encryptResults = new List<EncryptResult>
                    {
                            new EncryptResult
                            {
                                Hash = "SampleHashOne",
                                Salt = "SampleSaltOne"
                            },
                            new EncryptResult
                            {
                                Hash = "SampleHashTwo",
                                Salt = "SampleSaltTwo"
                            }
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
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
                        () =>
                        {
                            var result = encryptResults[encryptResultsIndex];
                            encryptResultsIndex++;
                            return result;
                        });

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                    //Assert
                    passwordEncryptionServiceMock
                    .Verify(
                        passwordEncryptionService => passwordEncryptionService.Encrypt
                        (
                           newPassword,
                            null
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_ExistingPasswordsMatch_CallsUpdatedPasswordAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var account = new Abstractions.Account
                    {
                        AccountId = accountId,
                        PasswordHash = "SampleHashOne",
                        Salt = "SampleSaltOne"
                    };

                    var encryptResultsIndex = 0;
                    var encryptResults = new List<EncryptResult>
                    {
                            new EncryptResult
                            {
                                Hash = "SampleHashOne",
                                Salt = "SampleSaltOne"
                            },
                            new EncryptResult
                            {
                                Hash = "SampleHashTwo",
                                Salt = "SampleSaltTwo"
                            }
                    };

                    var accountIdObserved = (int?)null;
                    var passwordHashObserved = (string)null;
                    var saltObserved = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
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
                        () =>
                        {
                            var result = encryptResults[encryptResultsIndex];
                            encryptResultsIndex++;
                            return result;
                        });

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Callback((int accountId, string passwordHash, string salt) =>
                    {
                        accountIdObserved = accountId;
                        passwordHashObserved = passwordHash;
                        saltObserved = salt;
                    })
                    .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                    //Assert
                    Assert.IsNotNull(accountIdObserved);

                    accountDBServiceMock
                    .Verify(
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            (int)accountIdObserved,
                            passwordHashObserved,
                            saltObserved
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(accountId, (int)accountIdObserved);
                    Assert.AreEqual(encryptResults[1].Hash, passwordHashObserved);
                    Assert.AreEqual(encryptResults[1].Salt, saltObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_Successful_ReturnSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var accountId = 1;
                    string existingPassword = "SampleExistingPassword";
                    string newPassword = "SampleNewPassword";
                    var account = new Abstractions.Account
                    {
                        AccountId = accountId,
                        PasswordHash = "SampleHashOne",
                        Salt = "SampleSaltOne"
                    };

                    var encryptResultsIndex = 0;
                    var encryptResults = new List<EncryptResult>
                    {
                            new EncryptResult
                            {
                                Hash = "SampleHashOne",
                                Salt = "SampleSaltOne"
                            },
                            new EncryptResult
                            {
                                Hash = "SampleHashTwo",
                                Salt = "SampleSaltTwo"
                            }
                    };

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
                            (
                                It.IsAny<int>()
                            )
                        )
                        .ReturnsAsync
                        (
                            account
                        );

                    var passwordEncryptionServiceMock = serviceProvider.GetMock<IPasswordEncryptionService>();
                    passwordEncryptionServiceMock
                    .Setup
                    (
                        encryptionService => encryptionService.Encrypt
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Returns
                    (
                        () =>
                        {
                            var result = encryptResults[encryptResultsIndex];
                            encryptResultsIndex++;
                            return result;
                        });


                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

                    //Assert
                    Assert.AreEqual(UpdatePasswordResult.Successful, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        #endregion

        #region ResetPasswordAsync

        [TestMethod]
        public async Task ResetPasswordAsync_Runs_CallsSelectAccountByAccountIdAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    string newPassword = "SampleNewPassword";
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    var accountId = (int?)null;

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        accountId
                    );

                    //Act
                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

                    //Assert
                    accountDBServiceMock
                    .Verify
                    (
                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
                        (
                            token
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_TokenIsInvaild_ReturnsTokenInvaild()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    string newPassword = "SampleNewPassword";
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    var accountId = (int?)null;

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        accountId
                    );

                    //Act
                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

                    //Assert
                    Assert.AreEqual(ResetPasswordResult.TokenInvaild, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_AccountSelected_CallEncryptNewPassword()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    var accountId = 1;
                    string newPassword = "SampleNewPassword";
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };
                    var passwordObserved = (string)null;
                    var hashObserved = (string)null;

                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceStub
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            accountId
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
                        .Callback((string password, string hash) =>
                        {
                            passwordObserved = password;
                            hashObserved = hash;
                        })
                        .Returns
                        (
                            encryptResult
                        );

                    accountDBServiceStub
                    .Setup
                    (
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

                    //Assert
                    passwordEncryptionServiceMock
                    .Verify(
                        passwordEncryptionService => passwordEncryptionService.Encrypt
                        (
                            passwordObserved,
                            hashObserved
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(newPassword, passwordObserved);
                    Assert.IsNull(null, hashObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_AccountSelected_CallUpdatePassword()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    var accountId = 1;
                    string newPassword = "SampleNewPassword";
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };

                    var accountIdObserved = (int?)null;
                    var passwordHashObserved = (string)null;
                    var saltObserved = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            accountId
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

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Callback((int accountId, string passwordHash, string salt) =>
                    {
                        accountIdObserved = accountId;
                        passwordHashObserved = passwordHash;
                        saltObserved = salt;
                    })
                    .Returns(Task.CompletedTask);

                        //Act
                        var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

                        //Assert
                        accountDBServiceMock
                        .Verify(
                            accountDBService => accountDBService.UpdatePasswordAsync
                            (
                                accountId,
                                encryptResult.Hash,
                                encryptResult.Salt
                            ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_Successful_ReturnsSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var token = "SampleToken";
                    var accountId = 1;
                    string newPassword = "SampleNewPassword";
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };

                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceStub
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            accountId
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

                    accountDBServiceStub
                    .Setup
                    (
                        accountDBService => accountDBService.UpdatePasswordAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

                    //Assert
                    Assert.AreEqual(ResetPasswordResult.Successful, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region RequestPasswordResetEmailAsync

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_Runs_CallsSelectAccountByEmailAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByEmailAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        (Abstractions.Account)null
                    );

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    accountDBServiceMock
                    .Verify
                    (
                        accountDBService => accountDBService.SelectAccountByEmailAsync
                        (
                            email
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_AccountIsNull_ReturnsEmailNotFound()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.SelectAccountByEmailAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        (Abstractions.Account)null
                    );

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    Assert.AreEqual(RequestPasswordResetEmailResult.EmailNotFound, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailActivated_ReturnsEmailActivated()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new Abstractions.Account
                            {
                                Username = "",
                                EmailActivated = false
                            }
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();

                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (
                           new System.Guid()
                        );

                    accountDBServiceMock
                      .Setup
                      (
                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
                          (
                              It.IsAny<int>(),
                              It.IsAny<string>()
                          )
                      )
                      .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    Assert.AreEqual(RequestPasswordResetEmailResult.EmailNotActivated, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailPreferenceNone_ReturnsNoEmailSentDueToEmailPreference()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new Abstractions.Account
                            {
                                Username = "",
                                EmailActivated = true,
                                EmailPreference = EmailPreference.None
                            }
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();

                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (
                           new System.Guid()
                        );

                    accountDBServiceMock
                      .Setup
                      (
                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
                          (
                              It.IsAny<int>(),
                              It.IsAny<string>()
                          )
                      )
                      .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    Assert.AreEqual(RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_AccountExist_NewGuidCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;

                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new Abstractions.Account
                            {
                                Username = "",
                                EmailActivated = true,
                                EmailPreference = EmailPreference.Any
                            }
                        );

                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();

                    guidServiceMock
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (
                           new System.Guid()
                        );

                    accountDBServiceMock
                      .Setup
                      (
                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
                          (
                              It.IsAny<int>(),
                              It.IsAny<string>()
                          )
                      )
                      .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    guidServiceMock
                    .Verify(
                        guidService => guidService.NewGuid(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_AccountExist_InsertPasswordResetTokenAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;
                    var expectedAccount = new Abstractions.Account
                    {
                        AccountId = 1,
                        ActivateEmailToken = Guid.NewGuid(),
                        EmailPreferenceToken = Guid.NewGuid(),
                        Username = "",
                        EmailActivated = true,
                        EmailPreference = EmailPreference.Any
                    };
                    var expectedGuid = Guid.NewGuid();
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            expectedAccount
                        );

                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

                    guidServiceStub
                    .Setup
                    (
                        guidService => guidService.NewGuid()
                    )
                    .Returns
                    (
                        expectedGuid
                    );

                    accountDBServiceMock
                    .Setup
                    (
                        accountDBService => accountDBService.InsertPasswordResetTokenAsync
                        (
                            It.IsAny<int>(),
                            It.IsAny<string>()
                        )
                    )
                    .Returns(Task.CompletedTask);

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    accountDBServiceMock
                    .Verify
                    (
                        accountDBService => accountDBService.InsertPasswordResetTokenAsync
                        (
                            expectedAccount.AccountId,
                            expectedGuid.ToString()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_Successful_SendPasswordResetEmail()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;
                    var expectedAccount = new Abstractions.Account
                    {
                        AccountId = 1,
                        ActivateEmailToken = Guid.NewGuid(),
                        EmailPreferenceToken = Guid.NewGuid(),
                        Username = "",
                        EmailActivated = true,
                        EmailPreference = EmailPreference.Any
                    };
                    var passwordResetToken = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
                    accountDBServiceMock
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            expectedAccount
                        );

                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

                    guidServiceStub
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (
                           passwordResetToken
                        );

                    accountDBServiceMock
                      .Setup
                      (
                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
                          (
                              It.IsAny<int>(),
                              It.IsAny<string>()
                          )
                      )
                      .Returns(Task.CompletedTask);

                    var accountEmailServiceMock = serviceProvider.GetMock<IAccountEmailService>();

                    accountEmailServiceMock
                        .Setup
                        (
                            emailService => emailService.SendPasswordResetEmailAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        );



                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    accountEmailServiceMock
                    .Verify
                    (
                        emailService => emailService.SendPasswordResetEmailAsync
                        (
                            expectedAccount.Email,
                            passwordResetToken.ToString(),
                            expectedAccount.EmailPreferenceToken.ToString()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_Successful_ReturnsSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
                    var uutConcrete = (AccountManager)uut;
                    var email = (string)null;
                    var expectedAccount = new Abstractions.Account
                    {
                        AccountId = 1,
                        ActivateEmailToken = Guid.NewGuid(),
                        EmailPreferenceToken = Guid.NewGuid(),
                        Username = "",
                        EmailActivated = true,
                        EmailPreference = EmailPreference.Any
                    };
                    var expectedGuid = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

                    accountDBServiceStub
                        .Setup
                        (
                            accountDBService => accountDBService.SelectAccountByEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            expectedAccount
                        );

                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

                    guidServiceStub
                        .Setup
                        (
                            guidService => guidService.NewGuid()
                        )
                        .Returns
                        (
                           expectedGuid
                        );

                    accountDBServiceStub
                      .Setup
                      (
                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
                          (
                              It.IsAny<int>(),
                              It.IsAny<string>()
                          )
                      )
                      .Returns(Task.CompletedTask);

                    var accountEmailServiceMock = serviceProvider.GetMock<IAccountEmailService>();

                    accountEmailServiceMock
                    .Setup
                    (
                        emailService => emailService.SendPasswordResetEmailAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    );

                    //Act
                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

                    //Assert
                    Assert.AreEqual(RequestPasswordResetEmailResult.Successful, observed);
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
            serviceCollection.AddSingleton(Mock.Of<IEmailService>());
            
            var accountManagerOptions = new AccountManagerOptions
            {
                AdminToken = ADMIN_TOKEN
            };
            var options = Options.Create(accountManagerOptions);
            serviceCollection.AddSingleton<IOptions<AccountManagerOptions>>(options);
            return serviceCollection;
        }
        #endregion
    }
}
