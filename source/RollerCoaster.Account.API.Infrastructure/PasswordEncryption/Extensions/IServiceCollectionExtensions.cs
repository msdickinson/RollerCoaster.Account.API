using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption;

namespace RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddPasswordEncryption(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IPasswordEncryption, PasswordEncryption>();

            return serviceCollection;
        }
    }
}
