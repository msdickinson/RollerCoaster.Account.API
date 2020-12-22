using RollerCoaster.Account.API.Entities.Models;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models
{
    [ExcludeFromCodeCoverage]
    public class UserEntityDTO
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Username { get; set; }
        public string _etag { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public EmailPreference EmailPreference { get; set; }
        public string EmailPreferenceToken { get; set; }
        public bool EmailActivated { get; set; }
        public string ActivateEmailToken { get; set; }
        public Role Role { get; set; }
    }
}
