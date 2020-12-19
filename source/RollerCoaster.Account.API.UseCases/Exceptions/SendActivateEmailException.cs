using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.UseCases.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class SendActivateEmailException : Exception
    {
        public SendActivateEmailException(Exception innerException) : base("", innerException)
        {
            
        }
    }
}
