using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Configurators;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Redactor.Tests.Configurator
{

    [TestClass]
    public class AccountManagerOptionsConfiguratorTests : BaseTest
    {
        [TestMethod]
        public async Task Configure_Runs_DecryptCalled()
        {
            var accountManagerOptions = new AccountManagerOptions
            {
                AdminToken = "SampleAdminToken",
            };

            var accountManagerEncryptedOptions = new AccountManagerOptions
            {
                AdminToken = "SampleEncryptedAdminToken"
            };

            var configurationRoot = BuildConfigurationRoot(accountManagerOptions);
           

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var configurationEncryptionServiceMock = serviceProvider.GetMock<IConfigurationEncryptionService>();

                    configurationEncryptionServiceMock
                          .Setup
                          (
                              configurationEncryptionService => configurationEncryptionService.Decrypt
                              (
                                  accountManagerOptions.AdminToken
                              )
                          )
                          .Returns
                          (
                                  accountManagerEncryptedOptions.AdminToken
                          );
                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<AccountManagerOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(accountManagerEncryptedOptions.AdminToken, options.AdminToken);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<AccountManagerOptions>, AccountManagerOptionsConfigurator>();
            serviceCollection.AddSingleton(Mock.Of<IConfigurationEncryptionService>());

            return serviceCollection;
        }

        #endregion
    }
}
