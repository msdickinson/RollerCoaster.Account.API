using DickinsonBros.Cosmos;
using DickinsonBros.Cosmos.Models;
using DickinsonBros.Test;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.UserEntityRepository.Reader
{
    public class SampleCosmosServiceOptions : CosmosServiceOptions
    {
    }

    [TestClass]
    public class UserEntityRepositoryReaderTests : BaseTest
    {
        #region LoadAsync

        [TestMethod]
        public async Task LoadAsync_Runs_FetchAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    var containerMock = serviceProvider.GetMock<Container>();
                    var itemResponseMock = new Mock<ItemResponse<UserEntityDTO>>();
                    itemResponseMock
                    .Setup(itemResponse => itemResponse.Resource)
                    .Returns(userEntityDTO);

                    containerMock
                    .Setup
                    (
                        container => container.ReadItemAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        )
                    )
                    .ReturnsAsync(itemResponseMock.Object);

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        itemResponseMock.Object
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.LoadAsync(username).ConfigureAwait(false);

                    //Assert
                    cosmosServiceMock
                    .Verify
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            username,
                            username
                        ),
                        Times.Once
                    );
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        [TestMethod]
        public async Task LoadAsync_Runs_ReturnsUserEntityData()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    var containerMock = serviceProvider.GetMock<Container>();
                    var itemResponseMock = new Mock<ItemResponse<UserEntityDTO>>();
                    itemResponseMock
                    .Setup(itemResponse => itemResponse.Resource)
                    .Returns(userEntityDTO);

                    containerMock
                    .Setup
                    (
                        container => container.ReadItemAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        )
                    )
                    .ReturnsAsync(itemResponseMock.Object);

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        itemResponseMock.Object
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.LoadAsync(username).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(userEntityDTO.ActivateEmailToken, observed.ActivateEmailToken);
                    Assert.AreEqual(userEntityDTO.EmailActivated, observed.EmailActivated);
                    Assert.AreEqual(userEntityDTO.Email, observed.Email);
                    Assert.AreEqual(userEntityDTO.EmailPreference, observed.EmailPreference);
                    Assert.AreEqual(userEntityDTO.EmailPreferenceToken, observed.EmailPreferenceToken);
                    Assert.AreEqual(userEntityDTO.PasswordHash, observed.PasswordHash);
                    Assert.AreEqual(userEntityDTO.Role, observed.Role);
                    Assert.AreEqual(userEntityDTO.Salt, observed.Salt);
                    Assert.AreEqual(userEntityDTO.Username, observed.Username);
                    Assert.AreEqual(userEntityDTO._etag, observed.Version);
                    Assert.AreEqual(userEntityDTO.Id, observed.Username);
                    Assert.AreEqual(userEntityDTO.Key, observed.Username);

                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        #endregion

        #region UsernameExistsAsync

        [TestMethod]
        public async Task UsernameExistsAsync_Runs_FetchAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    var containerMock = serviceProvider.GetMock<Container>();
                    var itemResponseMock = new Mock<ItemResponse<UserEntityDTO>>();
                    itemResponseMock
                    .Setup(itemResponse => itemResponse.Resource)
                    .Returns(userEntityDTO);

                    containerMock
                    .Setup
                    (
                        container => container.ReadItemAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        )
                    )
                    .ReturnsAsync(itemResponseMock.Object);

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        itemResponseMock.Object
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.UsernameExistsAsync(username).ConfigureAwait(false);

                    //Assert
                    cosmosServiceMock
                    .Verify
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            username,
                            username
                        ),
                        Times.Once
                    );
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        [TestMethod]
        public async Task UsernameExistsAsync_Exists_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    var containerMock = serviceProvider.GetMock<Container>();
                    var itemResponseMock = new Mock<ItemResponse<UserEntityDTO>>();
                    itemResponseMock
                    .Setup(itemResponse => itemResponse.Resource)
                    .Returns(userEntityDTO);

                    containerMock
                    .Setup
                    (
                        container => container.ReadItemAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        )
                    )
                    .ReturnsAsync(itemResponseMock.Object);

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync
                    (
                        itemResponseMock.Object
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.UsernameExistsAsync(username).ConfigureAwait(false);

                    //Assert
                    Assert.IsTrue(observed);
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }
      
        [TestMethod]
        public async Task UsernameExistsAsync_DoesNotExist_ReturnsFalse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var username = "SampleUsername";

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    var cosmosExceptionNotFound = new CosmosException("", System.Net.HttpStatusCode.NotFound, 0, "", 0);

                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.FetchAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<string>()
                        )
                    )
                    .Throws(cosmosExceptionNotFound);

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.UsernameExistsAsync(username).ConfigureAwait(false);

                    //Assert
                    Assert.IsFalse(observed);
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        #endregion


        #region EmailExistsAsync

        [TestMethod]
        public async Task EmailExistsAsync_Runs_QueryAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@email.com";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };


                    var userEntityDTOs = new List<UserEntityDTO>
                    {
                        userEntityDTO
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.QueryAsync<UserEntityDTO>
                        (
                            It.IsAny<QueryDefinition>(),
                            It.IsAny<QueryRequestOptions>()
                        )
                    )
                    .ReturnsAsync
                    (
                        userEntityDTOs.AsEnumerable()
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.EmailExistsAsync(email).ConfigureAwait(false);

                    //Assert
                    cosmosServiceMock
                    .Verify
                    (
                        guidService => guidService.QueryAsync<UserEntityDTO>
                        (
                            It.IsAny<QueryDefinition>(),
                            It.IsAny<QueryRequestOptions>()
                        ),
                        Times.Once
                    );
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        [TestMethod]
        public async Task EmailExistsAsync_AtLeastOneEmailExists_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@email.com";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };


                    var userEntityDTOs = new List<UserEntityDTO>
                    {
                        userEntityDTO
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.QueryAsync<UserEntityDTO>
                        (
                            It.IsAny<QueryDefinition>(),
                            It.IsAny<QueryRequestOptions>()
                        )
                    )
                    .ReturnsAsync
                    (
                        userEntityDTOs.AsEnumerable()
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.EmailExistsAsync(email).ConfigureAwait(false);

                    //Assert
                    Assert.IsTrue(observed);
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        [TestMethod]
        public async Task EmailExistsAsync_NoEmailsExist_ReturnsFalse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@email.com";

                    var userEntityDTO = new UserEntityDTO
                    {
                        ActivateEmailToken = "SampleActivateEmailToken",
                        EmailActivated = false,
                        Email = "SampleEmail",
                        EmailPreference = EmailPreference.AccountOnly,
                        EmailPreferenceToken = "SampleEmailPreferenceToken",
                        PasswordHash = "SamplePasswordHash",
                        Role = Role.User,
                        Salt = "SampleSalt",
                        Username = "SampleUsername",
                        _etag = "SampleVersion",
                        Id = "SampleUsername",
                        Key = "SampleUsername",
                    };


                    var userEntityDTOs = new List<UserEntityDTO>
                    {
                        userEntityDTO
                    };

                    // Act
                    var userEntityData = userEntityDTO.ToUserEntityData();

                    //--Container                    

                    //--ICosmosService
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService<SampleCosmosServiceOptions>>();
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.QueryAsync<UserEntityDTO>
                        (
                            It.IsAny<QueryDefinition>(),
                            It.IsAny<QueryRequestOptions>()
                        )
                    )
                    .ReturnsAsync
                    (
                        userEntityDTOs.AsEnumerable()
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryReader>();
                    var uutConcrete = (UserEntityRepositoryReader<SampleCosmosServiceOptions>)uut;

                    //Act
                    var observed = await uutConcrete.EmailExistsAsync(email).ConfigureAwait(false);

                    //Assert
                    Assert.IsTrue(observed);
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }
        #endregion

        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddUserEntityRepositoryReader<SampleCosmosServiceOptions>();
            serviceCollection.AddSingleton(Mock.Of<ICosmosService<SampleCosmosServiceOptions>>());
            serviceCollection.AddSingleton(Mock.Of<Container>());

            return serviceCollection;
        }

        #endregion
    }
}
