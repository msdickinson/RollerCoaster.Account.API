using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.GuidFactory;

namespace RollerCoaster.Account.API.Infrastructure.Guid.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddGuidAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IGuidFactory, GuidAdapter>();

            return serviceCollection;
        }
    }
}
