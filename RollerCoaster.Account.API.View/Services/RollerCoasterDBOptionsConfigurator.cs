using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Telemetry.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Acccount.API.View.Models;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.View.Services
{
    [ExcludeFromCodeCoverage]
    public class RollerCoasterDBOptionsConfigurator : IConfigureOptions<RollerCoasterDBOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public RollerCoasterDBOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<RollerCoasterDBOptions>.Configure(RollerCoasterDBOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<StandardCertificateEncryptionServiceOptions>>();
            var dickinsonBrosDBOptions = configuration.GetSection(nameof(RollerCoasterDBOptions)).Get<RollerCoasterDBOptions>();
            dickinsonBrosDBOptions.ConnectionString = certificateEncryptionService.Decrypt(dickinsonBrosDBOptions.ConnectionString);
            configuration.Bind($"{nameof(TelemetryServiceOptions)}", options);

            options.ConnectionString = dickinsonBrosDBOptions.ConnectionString;
        }
    }
}
