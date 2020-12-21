using DickinsonBros.Cosmos.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUserEntityRepositoryReader<T>(this IServiceCollection serviceCollection)
            where T : CosmosServiceOptions
        {
            serviceCollection.TryAddSingleton<IUserEntityRepositoryReader, UserEntityRepositoryReader<T>>();

            return serviceCollection;
        }
    }
}
