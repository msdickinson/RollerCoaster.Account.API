using RollerCoaster.Account.API.Entities.Models;

namespace RollerCoaster.Account.API.Entities.User
{
    public interface IUserEntity
    {
        string ActivateEmailToken { get; }
        string Email { get; }
        EmailPreference EmailPreference { get; set; }
        string EmailPreferenceToken { get; }
        string PasswordHash { get; set; }
        Role Role { get; set; }
        string Salt { get; set; }
        string Username { get; }
        UserEntityData GetState();
    }
}