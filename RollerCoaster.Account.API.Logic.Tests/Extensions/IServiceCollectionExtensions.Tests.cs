using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Logic.Extensions;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Configurators;
using System.Linq;

namespace RollerCoaster.Account.API.Logic.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddDateTimeService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddAccountManager();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IAccountManager) &&
                                                       serviceDefinition.ImplementationType == typeof(AccountManager) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<AccountManagerOptions>) &&
                                           serviceDefinition.ImplementationType == typeof(AccountManagerOptionsConfigurator) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
