using DickinsonBros.Cosmos.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.UserEntityRepository.Reader.Extensions
{
    public class SampleCosmosServiceOptions : CosmosServiceOptions
    {
    }

    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddUseCases_Runs_UseCasesAddedToServiceCollection()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddUserEntityRepositoryReader<CosmosServiceOptions>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IUserEntityRepositoryReader) &&
                                           serviceDefinition.ImplementationType == typeof(UserEntityRepositoryReader<CosmosServiceOptions>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}