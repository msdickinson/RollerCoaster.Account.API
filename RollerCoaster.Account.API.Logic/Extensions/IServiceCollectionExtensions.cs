using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Configurators;

namespace RollerCoaster.Account.API.Logic.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountManager(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountManager, AccountManager>();
            serviceCollection.TryAddSingleton<IConfigureOptions<AccountManagerOptions>, AccountManagerOptionsConfigurator>();

            return serviceCollection;
        }
    }
}
