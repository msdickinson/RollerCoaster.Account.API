using DickinsonBros.Cosmos;
using DickinsonBros.Test;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;
using System.Threading;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.UserEntityRepository.Writer
{
    [TestClass]
    public class UserEntityRepositoryWriterTests : BaseTest
    {
        #region LoadAsync

        [TestMethod]
        public async Task SaveAsync_Runs_InsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var userEntityData = new UserEntityData
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
                        Version = "SampleVersion"
                    };

                    var userEntityDTO = userEntityData.ToUserEntityDTO();
                    userEntityDTO._etag = "1000";

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
                    var cosmosServiceMock = serviceProvider.GetMock<ICosmosService>();
                    var keyObserved = (string)null;
                    var userEntityDTOObserved = (UserEntityDTO)null;
                    cosmosServiceMock
                    .Setup
                    (
                        guidService => guidService.InsertAsync<UserEntityDTO>
                        (
                            It.IsAny<string>(),
                            It.IsAny<UserEntityDTO>()
                        )
                    )
                    .Callback((string key, UserEntityDTO userEntityDTOCB) =>
                    {
                        keyObserved = key;
                        userEntityDTOObserved = userEntityDTOCB;
                    })
                    .ReturnsAsync
                    (
                        itemResponseMock.Object
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IUserEntityRepositoryWriter>();
                    var uutConcrete = (UserEntityRepositoryWriter)uut;

                    //Act
                    var observed = await uutConcrete.SaveAsync(userEntityData).ConfigureAwait(false);

                    //Assert
                    cosmosServiceMock
                    .Verify
                    (
                        guidService => guidService.InsertAsync<UserEntityDTO>
                        (
                            userEntityData.Username,
                            userEntityDTOObserved
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(userEntityData.ActivateEmailToken   , observed.ActivateEmailToken);
                    Assert.AreEqual(userEntityData.EmailActivated       , observed.EmailActivated);
                    Assert.AreEqual(userEntityData.Email                , observed.Email);
                    Assert.AreEqual(userEntityData.EmailPreference      , observed.EmailPreference);
                    Assert.AreEqual(userEntityData.EmailPreferenceToken , observed.EmailPreferenceToken);
                    Assert.AreEqual(userEntityData.PasswordHash         , observed.PasswordHash);
                    Assert.AreEqual(userEntityData.Role                 , observed.Role);
                    Assert.AreEqual(userEntityData.Salt                 , observed.Salt);
                    Assert.AreEqual(userEntityData.Username             , observed.Username);
                    Assert.AreEqual("1000"                              , observed.Version);
                },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }


        #endregion

        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddUserEntityRepositoryWriter();
            serviceCollection.AddSingleton(Mock.Of<ICosmosService>());
            serviceCollection.AddSingleton(Mock.Of<Container>());

            return serviceCollection;
        }

        #endregion
    }
}
