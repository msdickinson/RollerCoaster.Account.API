using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using RollerCoaster.Account.API.View.Configurators;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountDBService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IAccountDBService, AccountDBService>();
            serviceCollection.TryAddSingleton<IConfigureOptions<AccountDBServiceOptions>, AccountDBServiceOptionsConfigurator>();

            return serviceCollection;
        }
    }
}
