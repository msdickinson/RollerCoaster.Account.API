using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Logic.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateAccountDescriptor
    {
        public CreateAccountResult Result { get; set; }
        public int? AccountId { get; set; }
    }
}
