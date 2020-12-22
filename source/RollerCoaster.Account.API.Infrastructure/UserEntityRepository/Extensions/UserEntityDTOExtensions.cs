using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions
{
    public static class UserEntityDTOExtensions
    {
        public static UserEntityData ToUserEntityData(this UserEntityDTO userEntityData)
        {
            return new UserEntityData
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
                Version = userEntityData._etag
            };
        }
    }
}
