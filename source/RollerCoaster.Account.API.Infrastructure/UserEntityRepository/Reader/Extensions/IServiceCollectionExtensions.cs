using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUserEntityRepositoryReader(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IUserEntityRepositoryReader, UserEntityRepositoryReader>();

            return serviceCollection;
        }
    }
}
