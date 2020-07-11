using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class ActivateEmailWithTokenRequest
    {
        public string ActivateEmailToken { get; set; }
    }
}
