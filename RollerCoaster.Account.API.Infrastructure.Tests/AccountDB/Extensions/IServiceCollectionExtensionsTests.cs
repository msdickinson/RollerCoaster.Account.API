using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Extensions;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using RollerCoaster.Account.API.View.Configurators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RollerCoaster.Account.API.Infrastructure.Tests.AccountDB.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddAccountDBService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddAccountDBService();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IAccountDBService) &&
                                           serviceDefinition.ImplementationType == typeof(AccountDBService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<AccountDBServiceOptions>) &&
                               serviceDefinition.ImplementationType == typeof(AccountDBServiceOptionsConfigurator) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
