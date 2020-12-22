using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.UseCases.Exceptions.InvaildRequestsException
{
    [ExcludeFromCodeCoverage]
    public class InvaildRequestData
    {
        public string PropertyName { get; set; }
        public string VaildationError { get; set; }
    }
}
