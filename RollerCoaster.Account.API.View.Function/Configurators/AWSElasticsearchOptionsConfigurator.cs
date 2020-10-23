using DickinsonBros.Encryption.Certificate.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.View.Function.Models;

namespace RollerCoaster.Account.API.View.Function.Configurators
{
    public class AWSElasticsearchOptionsConfigurator : IConfigureOptions<AWSElasticsearchOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AWSElasticsearchOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        void IConfigureOptions<AWSElasticsearchOptions>.Configure(AWSElasticsearchOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var configuration = provider.GetRequiredService<IConfiguration>();
                var configurationEncryptionService = provider.GetRequiredService<IConfigurationEncryptionService>();
                var awsElasticsearchOptions = configuration.GetSection(nameof(AWSElasticsearchOptions)).Get<AWSElasticsearchOptions>();

                configuration.Bind($"{nameof(AWSElasticsearchOptions)}", options);

                options.URL = configurationEncryptionService.Decrypt(awsElasticsearchOptions.URL);
            }
        }
    }
}
