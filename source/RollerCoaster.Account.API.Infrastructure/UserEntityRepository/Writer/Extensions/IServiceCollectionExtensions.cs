using DickinsonBros.Cosmos.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUserEntityRepositoryWriter<T>(this IServiceCollection serviceCollection)
        where T : CosmosServiceOptions
        {
            serviceCollection.TryAddSingleton<IUserEntityRepositoryWriter, UserEntityRepositoryWriter<T>>();

            return serviceCollection;
        }
    }
}
