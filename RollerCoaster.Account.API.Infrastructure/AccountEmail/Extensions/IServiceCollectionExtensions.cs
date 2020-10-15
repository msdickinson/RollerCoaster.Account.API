using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RollerCoaster.Account.API.Infrastructure.AccountEmail.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountEmailService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IAccountEmailService, AccountEmailService>();

            return serviceCollection;
        }
    }
}
