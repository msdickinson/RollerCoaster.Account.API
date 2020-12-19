using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.Guid;
using RollerCoaster.Account.API.Infrastructure.Guid.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.GuidFactory;
using System.Linq;

namespace RollerCoaster.Account.API.Infrastructure.Tests.Guid.Extensions
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
            serviceCollection.AddGuidAdapter();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IGuidFactory) &&
                                           serviceDefinition.ImplementationType == typeof(GuidAdapter) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}