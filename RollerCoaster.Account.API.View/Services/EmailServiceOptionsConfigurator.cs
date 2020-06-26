using DickinsonBros.Email.Models;
using DickinsonBros.Encryption.Certificate.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Acccount.API.View.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.View.Services
{
    [ExcludeFromCodeCoverage]
    public class EmailServiceOptionsConfigurator : IConfigureOptions<EmailServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public EmailServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<EmailServiceOptions>.Configure(EmailServiceOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<StandardCertificateEncryptionServiceOptions>>();
            var emaiServiceOptions = configuration.GetSection(nameof(EmailServiceOptions)).Get<EmailServiceOptions>();

            if (!String.IsNullOrWhiteSpace(emaiServiceOptions.Password))
            {
                emaiServiceOptions.Password = certificateEncryptionService.Decrypt(emaiServiceOptions.Password);
            }

            configuration.Bind($"{nameof(emaiServiceOptions)}", options);

            options.Host = emaiServiceOptions.Host;
            options.Password = emaiServiceOptions.Password;
            options.Port = emaiServiceOptions.Port;
            options.SaveDirectory = emaiServiceOptions.SaveDirectory;
            options.SaveEmail = emaiServiceOptions.SaveEmail;
            options.SendSmtp = emaiServiceOptions.SendSmtp;
            options.UserName = emaiServiceOptions.UserName;
            options.SmtpTimeoutSeconds = emaiServiceOptions.SmtpTimeoutSeconds;

        }
    }
}
