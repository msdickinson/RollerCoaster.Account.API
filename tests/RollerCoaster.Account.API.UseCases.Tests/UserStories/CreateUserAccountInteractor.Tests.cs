using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Entities.User;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email.Models;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.GuidFactory;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption.Models;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.UseCases.Tests.UserStories
{
    [TestClass]
    public class CreateUserAccountInteractorTests : BaseTest
    {
        #region CreateUserAccountAsync
        [TestMethod]
        public async Task CreateUserAccountAsync_VaildInput_ReturnsUsername()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Email = "SampleEmail@Email.com",
                        Password = "SamplePassword",
                        Username = "SampleUsername"
                    };

                    //--IPasswordEncryption
                    var passwordEncryptionMock = serviceProvider.GetMock<IPasswordEncryption>();
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };

                    passwordEncryptionMock
                    .Setup
                    (
                        passwordEncryption => passwordEncryption.Encrypt
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns
                    (
                        encryptResult
                    );
                    //--IGuidFactory
                    var guidFactoryMock = serviceProvider.GetMock<IGuidFactory>();
                    var guid = new Guid("11111111-1111-1111-1111-111111111111");

                    guidFactoryMock
                    .Setup
                    (
                        guidFactory => guidFactory.NewGuid()
                    )
                    .Returns
                    (
                        guid
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();

                    var usernameExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );


                    var emailExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        emailExists
                    );

                    //--IUserEntityRepositoryWriter
                    var userEntityRepositoryWriterMock = serviceProvider.GetMock<IUserEntityRepositoryWriter>();
                    var userEntityDataObserved = (UserEntityData)null;
                    var userEntityData = new UserEntityData
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        Email = "SampleEmail@Email.com",
                        EmailActivated = false,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        EmailPreference = EmailPreference.Any,
                        Role = Role.User,
                        Username = "SampleUsername",
                        PasswordHash = "SamplePasswordHash",
                        Salt = "SampleSalt",
                        Version = "SampleVersion"
                    };

                    userEntityRepositoryWriterMock
                    .Setup
                    (
                        userEntityRepositoryWriter => userEntityRepositoryWriter.SaveAsync
                        (
                            It.IsAny<UserEntityData>()
                        )
                    )
                    .Callback
                    (
                        (UserEntityData userEntityData) =>
                        {
                            userEntityDataObserved = userEntityData;
                        }
                    )

                    .ReturnsAsync
                    (
                        userEntityData
                    );

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    var sendActivateEmailRequestObserved = (SendActivateEmailRequest)null;

                    emailMock
                    .Setup
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        )
                    )
                    .Callback((SendActivateEmailRequest sendActivateEmailRequest) =>
                    {
                        sendActivateEmailRequestObserved = sendActivateEmailRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    var observed = await uutConcrete.CreateUserAccountAsync(createUserAccountRequest).ConfigureAwait(false);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(createUserAccountRequest.Username, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        #endregion

        #region VaildateRequestAsync
        [TestMethod]
        public async Task VaildatePassword_VaildInput_DoesNotThrow()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Email = "SampleEmail@Email.com",
                        Password = "SamplePassword",
                        Username = "SampleUsername"
                    };

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.VaildateRequestAsync(createUserAccountRequest).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }
        #endregion

        #region VaildateUsername
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "username")]
        public async Task VaildateUsername_UsernameIsNull_ThrowsArgumentNullException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var username = (string)null;

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.VaildateUsername(username);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task VaildateUsername_UsernameIsNotNull_DoesNotThrow()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var username = "SampleUsername";

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.VaildateUsername(username);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        #endregion

        #region VaildatePassword
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "password")]
        public async Task VaildatePassword_PasswordIsNull_ThrowsArgumentNullException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var password = (string)null;

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.VaildatePassword(password);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "password")]
        public async Task VaildatePassword_PasswordIsLessThen7Char_ThrowsArgumentException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var password = "123456";

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.VaildatePassword(password);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task VaildatePassword_PasswordIs7OrMoreChar_DoesNotThrow()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var password = "1234567";

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.VaildatePassword(password);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        #endregion

        #region VaildateEmailAsync

        [TestMethod]
        public async Task VaildateEmailAsync_EmailIsNull_VaildateEmailAsyncIsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var email = (string)null;

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    emailMock
                    .Setup
                    (
                        email => email.VaildateEmailAsync
                        (
                            It.IsAny<string>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.VaildateEmailAsync(email).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        email => email.VaildateEmailAsync
                        (
                            It.IsAny<string>()
                        ),
                        Times.Never
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task VaildateEmailAsync_EmailIsNotNull_VaildateEmailAsyncIsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var email = "SampleEmail@Email.com";

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    emailMock
                    .Setup
                    (
                        email => email.VaildateEmailAsync
                        (
                            It.IsAny<string>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.VaildateEmailAsync(email).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        emailService => emailService.VaildateEmailAsync
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
        [ExpectedException(typeof(InvaildEmailFormatException))]
        public async Task VaildateEmailAsync_InvaildEmailFormat_ThrowsInvaildEmailFormatException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var email = "NotVaildEmailFormat";

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    emailMock
                    .Setup
                    (
                        email => email.VaildateEmailAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ThrowsAsync(new InvaildEmailFormatException());

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.VaildateEmailAsync(email).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        [ExpectedException(typeof(InvaildEmailDomainException))]
        public async Task VaildateEmailAsync_InvaildEmailDomain_ThrowsInvaildEmailDomainException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var email = "Email@InvaildEmailDomain.com";

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    emailMock
                    .Setup
                    (
                        email => email.VaildateEmailAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ThrowsAsync(new InvaildEmailDomainException());

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.VaildateEmailAsync(email).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }
        #endregion

        #region CreateUserEntity

        [TestMethod]
        public async Task CreateUserEntity_Runs_EncryptCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Email = "SampleEmail@Email.com",
                        Password = "SamplePassword",
                        Username = "SampleUsername"
                    };

                    //--IPasswordEncryption
                    var passwordEncryptionMock = serviceProvider.GetMock<IPasswordEncryption>();
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };

                    passwordEncryptionMock
                    .Setup
                    (
                        passwordEncryption => passwordEncryption.Encrypt
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns
                    (
                        encryptResult
                    );
                    //--IGuidFactory
                    var guidFactoryMock = serviceProvider.GetMock<IGuidFactory>();
                    var guids = new List<Guid>
                    {
                        new Guid("11111111-1111-1111-1111-111111111111"),
                        new Guid("22222222-2222-2222-2222-222222222222"),
                        new Guid("33333333-3333-3333-3333-333333333333")
                    };
                    var callBackCount = 0;

                    guidFactoryMock
                    .Setup
                    (
                        guidFactory => guidFactory.NewGuid()
                    )
                    .Callback(() => {
                        callBackCount++;
                    })
                    .Returns
                    (
                        guids[callBackCount]
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.CreateUserEntity(createUserAccountRequest);

                    //Assert
                    passwordEncryptionMock
                    .Verify
                    (
                        passwordEncryption => passwordEncryption.Encrypt
                        (
                            createUserAccountRequest.Password
                        ),
                        Times.Once
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task CreateUserEntity_Runs_NewGuidCalledTwoTimes()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Email = "SampleEmail@Email.com",
                        Password = "SamplePassword",
                        Username = "SampleUsername"
                    };

                    //--IPasswordEncryption
                    var passwordEncryptionMock = serviceProvider.GetMock<IPasswordEncryption>();
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };

                    passwordEncryptionMock
                    .Setup
                    (
                        passwordEncryption => passwordEncryption.Encrypt
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns
                    (
                        encryptResult
                    );
                    //--IGuidFactory
                    var guidFactoryMock = serviceProvider.GetMock<IGuidFactory>();
                    var guid = new Guid("11111111-1111-1111-1111-111111111111");

                    guidFactoryMock
                    .Setup
                    (
                        guidFactory => guidFactory.NewGuid()
                    )
                    .Returns
                    (
                        guid
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    uutConcrete.CreateUserEntity(createUserAccountRequest);

                    //Assert
                    guidFactoryMock
                    .Verify
                    (
                        guidFactory => guidFactory.NewGuid(),
                        Times.Exactly(2)
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task CreateUserEntity_Runs_ReturnsUserEntity()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var createUserAccountRequest = new CreateUserAccountRequest
                    {
                        Email = "SampleEmail@Email.com",
                        Password = "SamplePassword",
                        Username = "SampleUsername"
                    };

                    //--IPasswordEncryption
                    var passwordEncryptionMock = serviceProvider.GetMock<IPasswordEncryption>();
                    var encryptResult = new EncryptResult
                    {
                        Hash = "SampleHash",
                        Salt = "SampleSalt"
                    };

                    passwordEncryptionMock
                    .Setup
                    (
                        passwordEncryption => passwordEncryption.Encrypt
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns
                    (
                        encryptResult
                    );
                    //--IGuidFactory
                    var guidFactoryMock = serviceProvider.GetMock<IGuidFactory>();
                    var guid = new Guid("11111111-1111-1111-1111-111111111111");

                    guidFactoryMock
                    .Setup
                    (
                        guidFactory => guidFactory.NewGuid()
                    )
                    .Returns
                    (
                        guid
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    var observed = uutConcrete.CreateUserEntity(createUserAccountRequest);

                    //Assert
                    Assert.AreEqual(guid.ToString()                         , observed.ActivateEmailToken);
                    Assert.AreEqual(createUserAccountRequest.Email          , observed.Email);
                    Assert.AreEqual(false                                   , observed.EmailActivated);
                    Assert.AreEqual(EmailPreference.Any                     , observed.EmailPreference);
                    Assert.AreEqual(guid.ToString()                         , observed.EmailPreferenceToken);
                    Assert.AreEqual(encryptResult.Hash                      , observed.PasswordHash);
                    Assert.AreEqual(Role.User                               , observed.Role);
                    Assert.AreEqual(encryptResult.Salt                      , observed.Salt);
                    Assert.AreEqual(createUserAccountRequest.Username       , observed.Username);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        //EncryptPasswordCalled 

        //NewGuidCalled 3 Times

        //Return

        #endregion

        #region SaveUserAsync

        [TestMethod]
        public async Task SaveUserAsync_Runs_UsernameExistsAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = null,
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IUserEntityRepositoryReader
                    var usernameExists = true;
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );
                   
                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    try
                    {
                        await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);
                    }
                    catch(Exception ex)
                    {

                    };

                    //Assert
                    userEntityRepositoryReaderMock
                    .Verify
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            userEntity.Username
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateUsernameException))]
        public async Task SaveUserAsync_DuplicateUsername_ThrowsDuplicateUsernameException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = null,
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IUserEntityRepositoryReader
                    var usernameExists = true;
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);
             
                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SaveUserAsync_UnquieUsernameEmailIsNotNull_EmailExistsAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = "SampleEmail@Email.com",
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();

                    var usernameExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );


                    var emailExists = true;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        emailExists
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    try
                    {

                        await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);
                    }
                    catch(Exception ex)
                    {

                    }

                    //Assert
                    userEntityRepositoryReaderMock
                    .Verify
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            userEntity.Email
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SaveUserAsync_UnquieUsernameEmailNull_EmailExistsAsyncNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = null,
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();

                    var usernameExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );

                  
                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    try
                    {

                        await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {

                    }

                    //Assert
                    userEntityRepositoryReaderMock
                    .Verify
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            userEntity.Email
                        ),
                        Times.Never
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateEmailException))]
        public async Task SaveUserAsync_UnquieUsernameDuplicateEmail_ThrowsDuplicateEmailException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = "SampleEmail@Email.com",
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();

                    var usernameExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );


                    var emailExists = true;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        emailExists
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SaveUserAsync_UnquieUsernameAndEmail_SaveAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntityData = new UserEntityData
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        Email = "SampleEmail@Email.com",
                        EmailActivated = false,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        EmailPreference = EmailPreference.Any,
                        Role = Role.User,
                        Username = "SampleUsername",
                        PasswordHash = "SamplePasswordHash",
                        Salt = "SampleSalt",
                        Version = "SampleVersion"
                    };

                    var userEntity = new UserEntity
                    (
                        userEntityData
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();

                    var usernameExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );


                    var emailExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        emailExists
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryWriterMock = serviceProvider.GetMock<IUserEntityRepositoryWriter>();
                    var userEntityDataObserved = (UserEntityData)null;

                    userEntityRepositoryWriterMock
                    .Setup
                    (
                        userEntityRepositoryWriter => userEntityRepositoryWriter.SaveAsync
                        (
                            It.IsAny<UserEntityData>()
                        )
                    )
                    .Callback
                    (
                        (UserEntityData userEntityData) =>
                        {
                            userEntityDataObserved = userEntityData;
                        }
                    )

                    .ReturnsAsync
                    (
                        userEntityData
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    userEntityRepositoryWriterMock
                    .Verify
                    (
                        userEntityRepositoryWriter => userEntityRepositoryWriter.SaveAsync
                        (
                            It.IsAny<UserEntityData>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(userEntityData.ActivateEmailToken       , userEntityDataObserved.ActivateEmailToken);
                    Assert.AreEqual(userEntityData.Email                    , userEntityDataObserved.Email);
                    Assert.AreEqual(userEntityData.EmailActivated           , userEntityDataObserved.EmailActivated);
                    Assert.AreEqual(userEntityData.EmailPreference          , userEntityDataObserved.EmailPreference);
                    Assert.AreEqual(userEntityData.EmailPreferenceToken     , userEntityDataObserved.EmailPreferenceToken);
                    Assert.AreEqual(userEntityData.PasswordHash             , userEntityDataObserved.PasswordHash);
                    Assert.AreEqual(userEntityData.Role                     , userEntityDataObserved.Role);
                    Assert.AreEqual(userEntityData.Salt                     , userEntityDataObserved.Salt);
                    Assert.AreEqual(userEntityData.Username                 , userEntityDataObserved.Username);
                    Assert.AreEqual(userEntityData.Version                  , userEntityDataObserved.Version);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SaveUserAsync_UnquieUsernameAndEmail_ReturnUserEntityData()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntityData = new UserEntityData
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        Email = "SampleEmail@Email.com",
                        EmailActivated = false,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        EmailPreference = EmailPreference.Any,
                        Role = Role.User,
                        Username = "SampleUsername",
                        PasswordHash = "SamplePasswordHash",
                        Salt = "SampleSalt",
                        Version = "SampleVersion"
                    };

                    var userEntity = new UserEntity
                    (
                        userEntityData
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryReaderMock = serviceProvider.GetMock<IUserEntityRepositoryReader>();

                    var usernameExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.UsernameExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        usernameExists
                    );


                    var emailExists = false;
                    userEntityRepositoryReaderMock
                    .Setup
                    (
                        userEntityRepositoryReader => userEntityRepositoryReader.EmailExistsAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        emailExists
                    );

                    //--IUserEntityRepositoryReader
                    var userEntityRepositoryWriterMock = serviceProvider.GetMock<IUserEntityRepositoryWriter>();
                    var userEntityDataObserved = (UserEntityData)null;

                    userEntityRepositoryWriterMock
                    .Setup
                    (
                        userEntityRepositoryWriter => userEntityRepositoryWriter.SaveAsync
                        (
                            It.IsAny<UserEntityData>()
                        )
                    )
                    .Callback
                    (
                        (UserEntityData userEntityData) =>
                        {
                            userEntityDataObserved = userEntityData;
                        }
                    )

                    .ReturnsAsync
                    (
                        userEntityData
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    var observed = await uutConcrete.SaveUserAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    userEntityRepositoryWriterMock
                    .Verify
                    (
                        userEntityRepositoryWriter => userEntityRepositoryWriter.SaveAsync
                        (
                            It.IsAny<UserEntityData>()
                        ),
                        Times.Once
                    );

                    var observedState = observed.GetState();

                    Assert.AreEqual(userEntityData.ActivateEmailToken       , observedState.ActivateEmailToken);
                    Assert.AreEqual(userEntityData.Email                    , observedState.Email);
                    Assert.AreEqual(userEntityData.EmailActivated           , observedState.EmailActivated);
                    Assert.AreEqual(userEntityData.EmailPreference          , observedState.EmailPreference);
                    Assert.AreEqual(userEntityData.EmailPreferenceToken     , observedState.EmailPreferenceToken);
                    Assert.AreEqual(userEntityData.PasswordHash             , observedState.PasswordHash);
                    Assert.AreEqual(userEntityData.Role                     , observedState.Role);
                    Assert.AreEqual(userEntityData.Salt                     , observedState.Salt);
                    Assert.AreEqual(userEntityData.Username                 , observedState.Username);
                    Assert.AreEqual(userEntityData.Version                  , observedState.Version);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        #endregion

        #region SendActivateEmailAsync

        [TestMethod]
        public async Task SendActivateEmailAsync_EmailIsNull_SendActivateEmailAsyncIsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = null,
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();

                    emailMock
                    .Setup
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SendActivateEmailAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        ),
                        Times.Never
                    );


                },
                    serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SendActivateEmailAsync_IsActivated_SendActivateEmailAsyncIsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = "SampleEmail@Email.com",
                            EmailActivated = true,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    emailMock
                    .Setup
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SendActivateEmailAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        ),
                        Times.Never
                    );

                },
                    serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SendActivateEmailAsync_EmailPreferenceIsNotAnyOrAccountOnly_SendActivateEmailAsyncIsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = "SampleEmail@Email.com",
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.None,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    emailMock
                    .Setup
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SendActivateEmailAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        ),
                        Times.Never
                    );

                },
                    serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SendActivateEmailAsync_EmailExistIsNotActivatedEmailPreferenceIsAccountOnly_SendActivateEmailAsyncIsCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = "SampleEmail@Email.com",
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.AccountOnly,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    var sendActivateEmailRequestObserved = (SendActivateEmailRequest)null;

                    emailMock
                    .Setup
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        )
                    )
                    .Callback((SendActivateEmailRequest sendActivateEmailRequest) =>
                    {
                        sendActivateEmailRequestObserved = sendActivateEmailRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SendActivateEmailAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        email => email.SendActivateEmailAsync
                        (
                            sendActivateEmailRequestObserved
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(userEntity.ActivateEmailToken       , sendActivateEmailRequestObserved.ActivateToken);
                    Assert.AreEqual(userEntity.Email                    , sendActivateEmailRequestObserved.Email);
                    Assert.AreEqual(userEntity.EmailPreferenceToken     , sendActivateEmailRequestObserved.EmailPreferenceToken);
                    Assert.AreEqual(userEntity.Username                 , sendActivateEmailRequestObserved.Username);

                },
                    serviceCollection => ConfigureServices(serviceCollection)
            );

        }

        [TestMethod]
        public async Task SendActivateEmailAsync_EmailExistIsNotActivatedEmailPreferenceIsAny_SendActivateEmailAsyncIsCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var userEntity = new UserEntity
                    (
                        new UserEntityData
                        {
                            ActivateEmailToken = "SampleActivateEmailToken",
                            Email = "SampleEmail@Email.com",
                            EmailActivated = false,
                            EmailPreferenceToken = "SampleEmailPreferenceToken",
                            EmailPreference = EmailPreference.Any,
                            Role = Role.User,
                            Username = "SampleUsername",
                            PasswordHash = "SamplePasswordHash",
                            Salt = "SampleSalt",
                            Version = "SampleVersion"
                        }
                    );

                    //--IEmail
                    var emailMock = serviceProvider.GetMock<IEmail>();
                    var sendActivateEmailRequestObserved = (SendActivateEmailRequest)null;

                    emailMock
                    .Setup
                    (
                        email => email.SendActivateEmailAsync
                        (
                            It.IsAny<SendActivateEmailRequest>()
                        )
                    )
                    .Callback((SendActivateEmailRequest sendActivateEmailRequest) =>
                    {
                        sendActivateEmailRequestObserved = sendActivateEmailRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetService<ICreateUserAccountInteractor>();
                    var uutConcrete = (CreateUserAccountInteractor)uut;

                    //Act
                    await uutConcrete.SendActivateEmailAsync(userEntity).ConfigureAwait(false);

                    //Assert
                    emailMock
                    .Verify
                    (
                        email => email.SendActivateEmailAsync
                        (
                            sendActivateEmailRequestObserved
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(userEntity.ActivateEmailToken, sendActivateEmailRequestObserved.ActivateToken);
                    Assert.AreEqual(userEntity.Email, sendActivateEmailRequestObserved.Email);
                    Assert.AreEqual(userEntity.EmailPreferenceToken, sendActivateEmailRequestObserved.EmailPreferenceToken);
                    Assert.AreEqual(userEntity.Username, sendActivateEmailRequestObserved.Username);

                },
                    serviceCollection => ConfigureServices(serviceCollection)
            );

        }


        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICreateUserAccountInteractor, CreateUserAccountInteractor>();
            serviceCollection.AddSingleton(Mock.Of<IEmail>());
            serviceCollection.AddSingleton(Mock.Of<IPasswordEncryption>());
            serviceCollection.AddSingleton(Mock.Of<IGuidFactory>());
            serviceCollection.AddSingleton(Mock.Of<IUserEntityRepositoryReader>());
            serviceCollection.AddSingleton(Mock.Of<IUserEntityRepositoryWriter>());

            return serviceCollection;
        }

        #endregion

    }
}
