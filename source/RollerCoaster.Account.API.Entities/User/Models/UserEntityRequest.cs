using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Entities.Models
{
    [ExcludeFromCodeCoverage]
    public class UserEntityRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string ActivateEmailToken { get; set; }
        public string EmailPreferenceToken { get; set; }
        public bool EmailActivated { get; set; }
        public Role Role { get; set; }
        public EmailPreference EmailPreference { get; set; }
    }
}
