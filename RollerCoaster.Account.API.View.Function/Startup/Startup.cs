using Dickinsonbros.Middleware.Function.Extensions;
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
using DickinsonBros.SQL.Extensions;
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
using RollerCoaster.Account.API.Infrastructure.AccountDB.Extensions;
using RollerCoaster.Account.API.Infrastructure.AccountEmail.Extensions;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions;
using RollerCoaster.Account.API.Logic.Extensions;
using RollerCoaster.Account.API.View.Function.Configurators;
using RollerCoaster.Account.API.View.Function.Models;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
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
        const string FUNCTION_ENVIRONMENT_NAME = "FUNCTION_ENVIRONMENT_NAME";
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = EnrichConfiguration(builder.Services);
            ConfigureServices(builder.Services, configuration);
        }
        private IConfiguration EnrichConfiguration(IServiceCollection serviceCollection)
        {
            var existingConfiguration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddConfiguration(existingConfiguration);
            var configTransform = $"appsettings.{System.Environment.GetEnvironmentVariable(FUNCTION_ENVIRONMENT_NAME)}.json";
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

            //Add Dickinsonbros Services
            services.AddGuidService();
            services.AddDateTimeService();
            services.AddStopwatchService();
            services.AddDataTableService();
            services.AddLoggingService();
            services.AddRedactorService();
            services.AddConfigurationEncryptionService();
            services.AddTelemetryService();
            services.AddSQLService();

            //Startup Appears to have a timing issue wiht DnsClient, This will inject IEmailService, but will leave that null
            //It will need to be handled another way, those methods will throw if used.
            services.TryAddSingleton<IEmailService, EmailService>();
            services.TryAddSingleton<ISmtpClient, SmtpClient>();
            services.TryAddSingleton<IConfigureOptions<EmailServiceOptions>, EmailServiceOptionsConfigurator>();
            services.TryAddSingleton<ILookupClient>((serviceProvider) =>
            {
                return new LookupClient();
            });
            services.TryAddSingleton<IFileSystem, FileSystem>();

            services.AddJWTService<RollerCoasterJWTServiceOptions>();
            services.AddMiddlwareService<RollerCoasterJWTServiceOptions>(configuration);

            //Add Local Services    
            services.AddAccountManager();
            services.AddAccountDBService();
            services.AddPasswordEncryptionService();
            services.AddAccountEmailService();

            //Configure Appliation
            services.AddOptions();
            AddLogging(services, configuration);

        }
        public void AddLogging(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfigureOptions<AWSElasticsearchOptions>, AWSElasticsearchOptionsConfigurator>();

            var serviceProvider = services.BuildServiceProvider();
            var awsElasticsearchOptions = serviceProvider.GetService<IOptions<AWSElasticsearchOptions>>().Value;

            Environment.SetEnvironmentVariable("AWS_REGION", awsElasticsearchOptions.AWSRegion);

            services.AddLogging(loggingBuilder =>
            {
                var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(awsElasticsearchOptions.URL))
                {
                    IndexFormat = awsElasticsearchOptions.IndexFormat,
                })
                .CreateLogger();

                loggingBuilder.AddSerilog
                (
                    logger,
                    dispose: true
                );
            });
        }
    }
}
