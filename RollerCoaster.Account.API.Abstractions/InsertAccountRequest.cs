
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class InsertAccountRequest
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public string ActivateEmailToken { get; set; }
        public string EmailPreferenceToken { get; set; }
        public EmailPreference EmailPreference { get; set; }
        public string Role { get; set; }

    }
}
