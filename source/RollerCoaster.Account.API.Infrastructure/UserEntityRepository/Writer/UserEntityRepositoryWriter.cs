using DickinsonBros.Cosmos;
using Microsoft.Azure.Cosmos;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;
using System;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Writer
{
    public class UserEntityRepositoryWriter : IUserEntityRepositoryWriter
    {
        internal readonly ICosmosService _cosmosService;

        public UserEntityRepositoryWriter
        (
            ICosmosService cosmosService
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
