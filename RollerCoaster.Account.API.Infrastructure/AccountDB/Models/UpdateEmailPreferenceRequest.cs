using RollerCoaster.Account.API.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class UpdateEmailPreferenceRequest
    {
        public int AccountId { get; set; }

        public EmailPreference EmailPreference { get; set; }
    }
}
