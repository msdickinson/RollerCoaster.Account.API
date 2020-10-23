using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.View.Function.Configurators;
using RollerCoaster.Account.API.View.Function.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Tests.Configurators
{
    [TestClass]
    public class AWSElasticsearchOptionsConfiguratorTests : BaseTest
    {
        [TestMethod]
        public async Task Configure_Runs_ConfigReturns()
        {
            var awsElasticsearchOptions = new AWSElasticsearchOptions
            {
                AWSRegion = "SampleAWSRegion",
                IndexFormat = "SampleIndexFormat",
                URL = "SampleURL"
            };

            var awsElasticsearchOptionsDecrypted = new AWSElasticsearchOptions
            {
                URL = "SampleDecryptedURL",
            };

            var configurationRoot = BuildConfigurationRoot(awsElasticsearchOptions);

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
                            awsElasticsearchOptions.URL
                        )
                    )
                    .Returns
                    (
                            awsElasticsearchOptionsDecrypted.URL
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<AWSElasticsearchOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(awsElasticsearchOptionsDecrypted.URL    , options.URL);
                    Assert.AreEqual(awsElasticsearchOptions.IndexFormat     , options.IndexFormat);
                    Assert.AreEqual(awsElasticsearchOptions.AWSRegion       , options.AWSRegion);

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
            serviceCollection.AddSingleton<IConfigureOptions<AWSElasticsearchOptions>, AWSElasticsearchOptionsConfigurator>();
            serviceCollection.AddSingleton(Mock.Of<IConfigurationEncryptionService>());

            return serviceCollection;
        }

        #endregion
    }
}
