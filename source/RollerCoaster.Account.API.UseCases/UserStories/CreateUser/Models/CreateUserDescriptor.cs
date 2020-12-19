using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateUserAccountDescriptor
    {
        public CreateUserAccountResult Result { get; set; }
        public string Error { get; set; }
        public Guid? UserId { get; set; }
    }
}
