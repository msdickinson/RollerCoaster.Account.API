using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class ActivateEmailWithTokenResult
    {
        public bool VaildToken { get; set; }
        public bool EmailWasAlreadyActivated { get; set; }
    }
}
