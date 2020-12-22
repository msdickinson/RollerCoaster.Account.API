using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.UseCases.Extensions;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;
using System.Linq;

namespace RollerCoaster.Account.API.UseCases.Tests.Extensions
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
            serviceCollection.AddUseCases();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICreateUserAccountInteractor) &&
                                           serviceDefinition.ImplementationType == typeof(CreateUserAccountInteractor) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
