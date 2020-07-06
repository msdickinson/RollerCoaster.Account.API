using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class LoginDescriptor
    {
        public LoginResult Result { get; set; }
        public int? AccountId { get; set; }
        public string Role { get; set; }
    }
}
