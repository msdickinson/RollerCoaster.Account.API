using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddPasswordEncryptionService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();

            return serviceCollection;
        }
    }
}
