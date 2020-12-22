using Dickinsonbros.Middleware.Function.Extensions;
using DickinsonBros.Cosmos.Extensions;
using DickinsonBros.DataTable.Extensions;
using DickinsonBros.DateTime.Extensions;
using DickinsonBros.Email;
using DickinsonBros.Email.Abstractions;
using DickinsonBros.Email.Configurators;
using DickinsonBros.Email.Models;
using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Encryption.JWT.Extensions;
using DickinsonBros.Guid.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Stopwatch.Extensions;
using DickinsonBros.Telemetry.Extensions;
using DnsClient;
using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Infrastructure.Email.Extensions;
using RollerCoaster.Account.API.Infrastructure.Guid.Extensions;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer.Extensions;
using RollerCoaster.Account.API.UseCases.Extensions;
using RollerCoaster.Account.API.View.Function.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;

[assembly: WebJobsStartup(typeof(RollerCoaster.Account.API.View.Function.Startup.Startup))]
namespace RollerCoaster.Account.API.View.Function.Startup
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        const string _siteRootPath = @"\home\site\wwwroot\";
        const string BUILD_CONFIGURATION = "BUILD_CONFIGURATION";
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = FetchConfiguration(builder.Services);
            ConfigureServices(builder.Services, configuration);
        }
        private IConfiguration FetchConfiguration(IServiceCollection serviceCollection)
        {
            var existingConfiguration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddConfiguration(existingConfiguration);
            var configTransform = $"appsettings.{System.Environment.GetEnvironmentVariable(BUILD_CONFIGURATION)}.json";
            var isCICD = !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), configTransform));
            var functionConfigurationRootPath = isCICD ? _siteRootPath : Directory.GetCurrentDirectory();
            var config =
                configurationBuilder
                .SetBasePath(functionConfigurationRootPath)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile(configTransform, false)
                .Build();
            serviceCollection.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));

            return config;
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            AddMiddleware(services, configuration);
            AddUseCases(services, configuration);
            AddInfrastructure(services, configuration);
            AddDickinsonBrosPackages(services, configuration);
        }

        private void AddMiddleware(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMiddlwareService<RollerCoasterJWTServiceOptions>(configuration);
        }
       
        private void AddUseCases(IServiceCollection services, IConfiguration configuration)
        {
            services.AddUseCases();
        }

        private void AddInfrastructure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddEmailAdapter();
            services.AddGuidAdapter();
            services.AddPasswordEncryption();
            services.AddUserEntityRepositoryReader<ReaderCosmosServiceOptions>();
            services.AddUserEntityRepositoryWriter<WriterCosmosServiceOptions>();
        }

        private void AddDickinsonBrosPackages(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddGuidService();
            services.AddDateTimeService();
            services.AddStopwatchService();
            services.AddDataTableService();
            services.AddLoggingService();
            services.AddRedactorService();
            services.AddConfigurationEncryptionService();
            services.AddTelemetryService();
            services.AddCosmosService<ReaderCosmosServiceOptions>();
            services.AddCosmosService<WriterCosmosServiceOptions>();
            services.AddJWTService<RollerCoasterJWTServiceOptions>();

            //Note: Azure Fucntion require a differnt DI Configuration
            AddEmailService(services, configuration);
        }

        private void AddEmailService(IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IEmailService, EmailService>();
            services.TryAddTransient<ISmtpClient>((serviceProvider) => { return new SmtpClient(); });
            services.TryAddSingleton<IConfigureOptions<EmailServiceOptions>, EmailServiceOptionsConfigurator>();
            services.TryAddTransient<ILookupClient>((serviceProvider) => { return new LookupClient(); });
            services.TryAddSingleton<IFileSystem, FileSystem>();
        }
    }
}
