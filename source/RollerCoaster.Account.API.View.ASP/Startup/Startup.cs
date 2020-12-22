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
using DickinsonBros.Middleware.ASP;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Stopwatch.Extensions;
using DickinsonBros.Telemetry.Extensions;
using DnsClient;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RollerCoaster.Account.API.Infrastructure.Email.Extensions;
using RollerCoaster.Account.API.Infrastructure.Guid.Extensions;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer.Extensions;
using RollerCoaster.Account.API.UseCases.Extensions;
using RollerCoaster.Account.API.View.ASP.Configurators;
using RollerCoaster.Account.API.View.ASP.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
namespace RollerCoaster.Account.API.View.ASP.Startup
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {

        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Configure Application 
            AddUseCases(services);
            AddInfrastructure(services);
            AddDickinsonBrosPackages(services);

            //Configure ASP           
            services.AddOptions();
            AddControllers(services);
            AddVersioning(services);
            AddAuthentication(services);
            services.AddAuthorization();
            AddSwagger(services);
        }

        private void AddUseCases(IServiceCollection services)
        {
            services.AddUseCases();
        }

        private void AddInfrastructure(IServiceCollection services)
        {
            services.AddEmailAdapter();
            services.AddGuidAdapter();
            services.AddPasswordEncryption();
            services.AddUserEntityRepositoryReader<ReaderCosmosServiceOptions>();
            services.AddUserEntityRepositoryWriter<WriterCosmosServiceOptions>();
        }

        private void AddDickinsonBrosPackages(IServiceCollection services)
        {
            services.AddGuidService();
            services.AddDateTimeService();
            services.AddStopwatchService();
            services.AddDataTableService();
            services.AddLoggingService();
            services.AddRedactorService();
            services.AddConfigurationEncryptionService();
            services.AddTelemetryService();
            services.AddJWTService<RollerCoasterJWTServiceOptions>();
            // services.AddEmailService();

            services.TryAddSingleton<IEmailService, EmailService>();
            services.TryAddTransient<ISmtpClient>((serviceProvider) => { return new SmtpClient(); });
            services.TryAddSingleton<IConfigureOptions<EmailServiceOptions>, EmailServiceOptionsConfigurator>();
            services.TryAddTransient<ILookupClient>((serviceProvider) => { return new LookupClient(); });
            services.TryAddSingleton<IFileSystem, FileSystem>();

            services.AddCosmosService<ReaderCosmosServiceOptions>();
            services.AddCosmosService<WriterCosmosServiceOptions>();
        }

        public void AddControllers(IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });
        }

        public void AddVersioning(IServiceCollection services)
        {
            services.AddApiVersioning();
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
        }

        public void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: null);
            services.AddTransient<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>();
        }

        public void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Account API", Version = "1" });
                options.DocInclusionPredicate((version, apiDescription) =>
                {
                    var apiVersionAttribute =
                        (ApiVersionAttribute)
                        apiDescription.ActionDescriptor
                                      .EndpointMetadata
                                      .FirstOrDefault
                                      (
                                        metaData => metaData.GetType().Equals(typeof(ApiVersionAttribute))
                                      );

                    if (apiVersionAttribute != null && apiVersionAttribute.Versions.Any(e => "v" + e.MajorVersion == version))
                    {
                        apiDescription.RelativePath = apiDescription.RelativePath.Replace("/v{version}", $"/{version}");
                        var versionParameter =
                            apiDescription.ParameterDescriptions.SingleOrDefault(p => p.Name == "version");

                        if (versionParameter != null)
                        {
                            apiDescription.ParameterDescriptions.Remove(versionParameter);
                        }
                        return true;
                    }

                    return false;
                });
                options.AddSecurityDefinition("Bearer",
                   new OpenApiSecurityScheme
                   {
                       In = ParameterLocation.Header,
                       Description = "Please enter into field the word 'Bearer' following by space and JWT",
                       Name = "Authorization",
                       Type = SecuritySchemeType.ApiKey
                   });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                     {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>() }
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<MiddlewareService>();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
