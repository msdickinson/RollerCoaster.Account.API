using DickinsonBros.Encryption.Certificate.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Logic.Models;

namespace RollerCoaster.Account.API.View.Configurators
{
    public class AccountManagerOptionsConfigurator : IConfigureOptions<AccountManagerOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AccountManagerOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<AccountManagerOptions>.Configure(AccountManagerOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var configurationEncryptionService = provider.GetRequiredService<IConfigurationEncryptionService>();
            var accountManagerOptions = configuration.GetSection(nameof(AccountManagerOptions)).Get<AccountManagerOptions>();

            configuration.Bind($"{nameof(AccountManagerOptions)}", options);

            options.AdminToken = configurationEncryptionService.Decrypt(accountManagerOptions.AdminToken);

        }
    }
}
