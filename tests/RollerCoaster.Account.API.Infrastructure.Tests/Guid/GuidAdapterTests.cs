using DickinsonBros.Guid.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Infrastructure.Guid;
using RollerCoaster.Account.API.Infrastructure.Guid.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.GuidFactory;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.Guid
{
    [TestClass]
    public class GuidAdapterTests : BaseTest
    {
        [TestMethod]
        public async Task NewGuid_Runs_ReturnsGuid()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--IGuidService
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();

                    var guid = new System.Guid("11111111-1111-1111-1111-111111111111");

                    guidServiceMock
                    .Setup
                    (
                        guidService => guidService.NewGuid()
                    )
                    .Returns
                    (
                        guid
                    );

                    //--uut
                    var uut = serviceProvider.GetService<IGuidFactory>();
                    var uutConcrete = (GuidAdapter)uut;

                    //Act
                    var observed = uutConcrete.NewGuid();

                    //Assert
                    Assert.AreEqual(guid, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
              },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddGuidAdapter();
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());

            return serviceCollection;
        }

        #endregion

    }
}
