using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.PasswordEncryption.Extensions
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
            serviceCollection.AddPasswordEncryption();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IPasswordEncryption) &&
                                           serviceDefinition.ImplementationType == typeof(Infrastructure.PasswordEncryption.PasswordEncryption) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}