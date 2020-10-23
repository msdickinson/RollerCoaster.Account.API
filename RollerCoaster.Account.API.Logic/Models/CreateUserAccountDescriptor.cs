using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Logic.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateUserAccountDescriptor
    {
        public CreateUserAccountResult Result { get; set; }
        public int? AccountId { get; set; }
    }
}
