using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption.Models
{
    [ExcludeFromCodeCoverage]
    public class EncryptResult
    {
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
