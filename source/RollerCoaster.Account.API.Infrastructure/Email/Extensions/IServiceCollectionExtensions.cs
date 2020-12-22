using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email;

namespace RollerCoaster.Account.API.Infrastructure.Email.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IEmail, EmailAdapter>();

            return serviceCollection;
        }
    }
}
