using DickinsonBros.DateTime.Extensions;
using DickinsonBros.Email.Extensions;
using DickinsonBros.Email.Models;
using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Encryption.Certificate.Models;
using DickinsonBros.Encryption.JWT.Extensions;
using DickinsonBros.Encryption.JWT.Models;
using DickinsonBros.Guid.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Middleware;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Redactor.Models;
using DickinsonBros.SQL.Extensions;
using DickinsonBros.Stopwatch.Extensions;
using DickinsonBros.Telemetry.Extensions;
using DickinsonBros.Telemetry.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RollerCoaster.Acccount.API.View.Models;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using RollerCoaster.Account.API.Infrastructure.AccountEmail;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption;
using RollerCoaster.Account.API.Logic;
using RollerCoaster.Account.API.View.Models;
using RollerCoaster.Account.API.View.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RollerCoaster.Acccount.API.View
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });

            //Add Guid Service
            services.AddGuidService();

            //Add DateTime Service
            services.AddDateTimeService();

            //Add Stopwatch Service
            services.AddStopwatchService();

            //Add Logging Service
            services.AddLoggingService();

            //Add Redactor Service
            services.AddRedactorService();
            services.Configure<RedactorServiceOptions>(Configuration.GetSection(nameof(RedactorServiceOptions)));

            //Add Certificate Encryption Service
            services.AddCertificateEncryptionService<CertificateEncryptionServiceOptions>();
            services.Configure<CertificateEncryptionServiceOptions<StandardCertificateEncryptionServiceOptions>>(Configuration.GetSection(nameof(StandardCertificateEncryptionServiceOptions)));

            //Add Telemetry Service
            services.AddTelemetryService();
            services.AddSingleton<IConfigureOptions<TelemetryServiceOptions>, TelemetryServiceOptionsConfigurator>();

            //Add SQLService
            services.AddSQLService();

            //Add EmailService
            services.AddEmailService();
            services.AddSingleton<IConfigureOptions<EmailServiceOptions>, EmailServiceOptionsConfigurator>();

            //Add JWTService Website
            services.AddJWTService<WebsiteJWTServiceOptions>();
            services.Configure<JWTServiceOptions<WebsiteJWTServiceOptions>>(Configuration.GetSection(nameof(WebsiteJWTServiceOptions)));

            //Add JWTService Administration WebSite
            services.AddJWTService<AdministrationWebSiteJWTServiceOptions>();
            services.Configure<JWTServiceOptions<AdministrationWebSiteJWTServiceOptions>>(Configuration.GetSection(nameof(AdministrationWebSiteJWTServiceOptions)));

            //Add Versioning
            services.AddApiVersioning();
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            //Add Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: null);
            services.AddTransient<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>();

            //Add Swagger
            AddSwagger(services);

            //Add             
            services.AddSingleton<IAccountManager, AccountManager>();
            services.AddSingleton<IAccountEmailService, AccountEmailService>();
            services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();
            services.AddSingleton<IAccountDBService, AccountDBService>();
            services.AddSingleton<IConfigureOptions<RollerCoasterDBOptions>, RollerCoasterDBOptionsConfigurator>();
        }
        public void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API", Version = "1" });
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
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
