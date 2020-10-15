using DickinsonBros.DataTable.Extensions;
using DickinsonBros.DateTime.Extensions;
using DickinsonBros.Email.Extensions;
using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Encryption.JWT.Extensions;
using DickinsonBros.Guid.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Middleware.ASP;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.SQL.Extensions;
using DickinsonBros.Stopwatch.Extensions;
using DickinsonBros.Telemetry.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Extensions;
using RollerCoaster.Account.API.Infrastructure.AccountEmail.Extensions;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions;
using RollerCoaster.Account.API.Logic.Extensions;
using RollerCoaster.Account.API.View.Configurators;
using RollerCoaster.Account.API.View.Models;
using Serilog;
using System.Collections.Generic;
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
            //Configure Appliation
            services.AddOptions();                                               //[X] 
            AddControllers(services);                                            //[X]    
            AddLogging(services);                                                //[ ] 
            AddVersioning(services);                                             //[X] 
            AddAuthentication(services);                                         //[ ] 
            AddSwagger(services);                                                //[X] 

            //Add Dickinsonbros Services
            services.AddGuidService();                                           //[X] 
            services.AddDateTimeService();                                       //[X] 
            services.AddStopwatchService();                                      //[X] 
            services.AddDataTableService();                                      //[X] 
            services.AddLoggingService();                                        //[X] 
            services.AddRedactorService();                                       //[X] 
            services.AddConfigurationEncryptionService();                        //[ ] 
            services.AddTelemetryService();                                      //[ ] 
            services.AddSQLService();                                            //[ ] 
            services.AddEmailService();                                          //[ ] 
            services.AddJWTService<RollerCoasterJWTServiceOptions>();            //[ ] 

            //Add MemoryCatche
            services.AddMemoryCache();                                           //[X] 

            //Add Local Services    
            services.AddAccountManager();                                        //[ ] 
            services.AddAccountDBService();                                      //[ ] 
            services.AddPasswordEncryptionService();                             //[ ] 
            services.AddAccountEmailService();                                   //[ ] 
        }

        public void AddControllers(IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });
        }

        public void AddLogging(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .CreateLogger();

                loggingBuilder.AddSerilog
                (
                    logger,
                    dispose: true
                );

                loggingBuilder.AddConfiguration(Configuration);
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
