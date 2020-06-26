using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class InsertAccountResult
    {
        public int AccountId { get; set; }
        public bool DuplicateUser { get; set; }
    }
}
