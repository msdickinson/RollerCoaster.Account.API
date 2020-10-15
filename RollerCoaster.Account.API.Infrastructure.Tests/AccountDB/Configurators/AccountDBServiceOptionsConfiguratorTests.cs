using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using RollerCoaster.Account.API.View.Configurators;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.AccountDB.Configurators
{
    [TestClass]
    public class AccountDBServiceOptionsConfiguratorTests : BaseTest
    {
        [TestMethod]
        public async Task Configure_Runs_ConfigReturns()
        {
            var accountDBServiceOptions = new AccountDBServiceOptions
            {
                ConnectionString = "SampleConnectionString"
            };

            var accountDBServiceOptionsDecrypted = new AccountDBServiceOptions
            {
                ConnectionString = "SampleDecryptedConnectionString"
            };

            var configurationRoot = BuildConfigurationRoot(accountDBServiceOptions);

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
                            accountDBServiceOptions.ConnectionString
                        )
                    )
                    .Returns
                    (
                        accountDBServiceOptionsDecrypted.ConnectionString
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<AccountDBServiceOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);
                    Assert.AreEqual(accountDBServiceOptionsDecrypted.ConnectionString, options.ConnectionString);

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
            serviceCollection.AddSingleton<IConfigureOptions<AccountDBServiceOptions>, AccountDBServiceOptionsConfigurator>();
            serviceCollection.AddSingleton(Mock.Of<IConfigurationEncryptionService>());

            return serviceCollection;
        }

        #endregion
    }
}
