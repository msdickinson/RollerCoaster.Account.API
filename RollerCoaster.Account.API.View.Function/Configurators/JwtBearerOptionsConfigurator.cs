using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.View.Function.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace RollerCoaster.Account.API.View.Function.Configurators
{
    [ExcludeFromCodeCoverage]
    public class JwtBearerOptionsConfigurator : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public JwtBearerOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }

        public void Configure(JwtBearerOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var jwtAuthorizeOptions = configuration.GetSection(nameof(JWTAuthorizeOptions)).Get<JWTAuthorizeOptions>();

            var storeLocation = jwtAuthorizeOptions.StoreLocation == "LocalMachine"
                                ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;

            var X509Certificate2 = GetX509Certificate2(jwtAuthorizeOptions.ThumbPrint, storeLocation);
            var X509SecurityKey = new Microsoft.IdentityModel.Tokens.X509SecurityKey(X509Certificate2);

            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = X509SecurityKey,
                ValidAudience = jwtAuthorizeOptions.VaildAudience,
                ValidIssuer = jwtAuthorizeOptions.VaildIssuer
            };
        }

        public X509Certificate2 GetX509Certificate2(string thumbPrint, StoreLocation storeLocation)
        {
            try
            {
                using var x509Store = new X509Store(StoreName.My, storeLocation);
                x509Store.Open(OpenFlags.ReadOnly);
                var certificateCollection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, false);
                if (certificateCollection.Count > 0)
                {
                    return certificateCollection[0];
                }
                else
                {
                    throw new Exception($"No certificate found for Thumbprint {thumbPrint} in location {storeLocation}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unhandled exception. Thumbprint: {thumbPrint}, Location: {storeLocation}", ex);
            }
        }
    }
}
