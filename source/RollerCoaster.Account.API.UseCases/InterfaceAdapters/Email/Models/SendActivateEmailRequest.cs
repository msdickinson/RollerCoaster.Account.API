using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email.Models
{
    [ExcludeFromCodeCoverage]
    public class SendActivateEmailRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string ActivateToken { get; set; }
        public string EmailPreferenceToken { get; set; }
    }
}
