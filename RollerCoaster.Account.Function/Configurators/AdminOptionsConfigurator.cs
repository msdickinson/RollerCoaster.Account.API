using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Telemetry.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Acccount.API.View.Models;
using RollerCoaster.Account.API.Logic.Models;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.View.Configurators
{
    [ExcludeFromCodeCoverage]
    public class AdminOptionsConfigurator : IConfigureOptions<AdminOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AdminOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<AdminOptions>.Configure(AdminOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<StandardCertificateEncryptionServiceOptions>>();
            var adminOptions = configuration.GetSection(nameof(AdminOptions)).Get<AdminOptions>();
            options.Token = certificateEncryptionService.Decrypt(adminOptions.Token);

            configuration.Bind($"{nameof(TelemetryServiceOptions)}", options);
        }
    }
}
