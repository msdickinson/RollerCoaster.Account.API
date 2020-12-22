using Microsoft.Extensions.DependencyInjection;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;

namespace RollerCoaster.Account.API.UseCases.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICreateUserAccountInteractor, CreateUserAccountInteractor>();

            return serviceCollection;
        }
    }
}
