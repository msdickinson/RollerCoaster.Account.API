using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.PasswordEncryption.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddPasswordEncryptionService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddPasswordEncryptionService();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IPasswordEncryptionService) &&
                                           serviceDefinition.ImplementationType == typeof(PasswordEncryptionService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
