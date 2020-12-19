using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.UserEntityRepository.Writer.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddUseCases_Runs_UseCasesAddedToServiceCollection()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddUserEntityRepositoryWriter();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IUserEntityRepositoryWriter) &&
                                           serviceDefinition.ImplementationType == typeof(UserEntityRepositoryWriter) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}