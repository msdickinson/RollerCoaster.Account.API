using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions
{
    public static class UserEntityDataExtensions
    {
        public static UserEntityDTO ToUserEntityDTO(this UserEntityData userEntityData)
        {
            return new UserEntityDTO
            {
                ActivateEmailToken = userEntityData.ActivateEmailToken,
                Email = userEntityData.Email,
                EmailActivated = userEntityData.EmailActivated,
                EmailPreference = userEntityData.EmailPreference,
                EmailPreferenceToken = userEntityData.EmailPreferenceToken,
                PasswordHash = userEntityData.PasswordHash,
                Role = userEntityData.Role,
                Salt = userEntityData.Salt,
                Username = userEntityData.Username,
                Id = userEntityData.Username,
                Key = userEntityData.Username,
                _etag = userEntityData.Version
            };
        }
    }
}
