using RollerCoaster.Account.API.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class UpdateEmailPreferenceWithTokenRequest
    {
        public string EmailPreferenceToken { get; set; }
        public EmailPreference EmailPreference { get; set; }
    }
}
