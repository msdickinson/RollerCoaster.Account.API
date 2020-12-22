using DickinsonBros.Cosmos;
using Microsoft.Azure.Cosmos;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;
using System.Linq;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader
{
    public class UserEntityRepositoryReader<T> : IUserEntityRepositoryReader
    {
        internal readonly ICosmosService<T> _cosmosService;

        public UserEntityRepositoryReader
        (
            ICosmosService<T> cosmosService
        )
        {
            _cosmosService = cosmosService;
        }

        public async Task<UserEntityData> LoadAsync(string username)
        {
            return (await _cosmosService
                            .FetchAsync<UserEntityDTO>(username, username)
                            .ConfigureAwait(false))
                            .Resource
                            .ToUserEntityData();
        }

        public async Task<bool> UsernameExistsAsync(string userId)
        {
            try
            {
                await _cosmosService
                   .FetchAsync<UserEntityDTO>(userId, userId)
                   .ConfigureAwait(false);

                return true;
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var fetchedQuerySampleModel = await _cosmosService.QueryAsync<UserEntityDTO>
            (
                new QueryDefinition("SELECT * FROM c where c.email = @email")
                    .WithParameter("@email", email),
                new Microsoft.Azure.Cosmos.QueryRequestOptions
                {
                    MaxItemCount = 1
                }
            ).ConfigureAwait(false);
            return fetchedQuerySampleModel.Any();
        }

    }
}
