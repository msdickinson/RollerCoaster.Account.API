using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.Email;
using RollerCoaster.Account.API.Infrastructure.Email.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.Email.Extensions
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
            serviceCollection.AddEmailAdapter();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IEmail) &&
                                           serviceDefinition.ImplementationType == typeof(EmailAdapter) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}