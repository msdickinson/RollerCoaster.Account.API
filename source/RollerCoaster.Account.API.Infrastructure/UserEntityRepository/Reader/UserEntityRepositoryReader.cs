﻿using DickinsonBros.Cosmos;
using Microsoft.Azure.Cosmos;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;
using System;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Reader
{
    public class UserEntityRepositoryReader : IUserEntityRepositoryReader
    {
        internal readonly ICosmosService _cosmosService;

        public UserEntityRepositoryReader
        (
            ICosmosService cosmosService
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
            await Task.CompletedTask.ConfigureAwait(false);
            return false;
        }

    }
}
