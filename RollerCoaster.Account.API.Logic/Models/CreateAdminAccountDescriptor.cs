using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Logic.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateAdminAccountDescriptor
    {
        public CreateAdminAccountResult Result { get; set; }
        public int? AccountId { get; set; }
    }
}
