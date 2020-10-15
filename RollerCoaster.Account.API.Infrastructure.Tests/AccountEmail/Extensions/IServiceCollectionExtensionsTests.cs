using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.AccountEmail;
using RollerCoaster.Account.API.Infrastructure.AccountEmail.Extensions;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.AccountEmail.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddAccountEmailService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddAccountEmailService();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IAccountEmailService) &&
                                           serviceDefinition.ImplementationType == typeof(AccountEmailService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
