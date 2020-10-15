using DickinsonBros.Encryption.Certificate.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.View.Configurators
{
    [ExcludeFromCodeCoverage]
    public class AccountDBServiceOptionsConfigurator : IConfigureOptions<AccountDBServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AccountDBServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<AccountDBServiceOptions>.Configure(AccountDBServiceOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var configurationEncryptionService = provider.GetRequiredService<IConfigurationEncryptionService>();
            var accountDBServiceOptions = configuration.GetSection(nameof(AccountDBServiceOptions)).Get<AccountDBServiceOptions>();
            accountDBServiceOptions.ConnectionString = configurationEncryptionService.Decrypt(accountDBServiceOptions.ConnectionString);
            configuration.Bind($"{nameof(AccountDBServiceOptions)}", options);

            options.ConnectionString = accountDBServiceOptions.ConnectionString;
        }
    }
}
