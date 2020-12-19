using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateUserAccountRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
