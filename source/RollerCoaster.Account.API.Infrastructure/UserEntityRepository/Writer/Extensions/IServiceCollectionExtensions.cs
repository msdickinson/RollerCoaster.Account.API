using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUserEntityRepositoryWriter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IUserEntityRepositoryWriter, UserEntityRepositoryWriter>();

            return serviceCollection;
        }
    }
}
