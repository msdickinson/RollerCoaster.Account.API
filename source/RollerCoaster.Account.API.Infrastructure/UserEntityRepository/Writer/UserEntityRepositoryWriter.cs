using DickinsonBros.Cosmos;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer
{
    public class UserEntityRepositoryWriter<T> : IUserEntityRepositoryWriter
    {
        internal readonly ICosmosService<T> _cosmosService;

        public UserEntityRepositoryWriter
        (
            ICosmosService<T> cosmosService
        )
        {
            _cosmosService = cosmosService;
        }
        public async Task<UserEntityData> SaveAsync(UserEntityData userEntityData)
        {
            var userEntityDTO = userEntityData.ToUserEntityDTO();

            return (await _cosmosService
                    .InsertAsync(userEntityDTO.Key, userEntityDTO)
                    .ConfigureAwait(false))
                    .Resource
                    .ToUserEntityData();
        }
    }
}
