using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Acccount.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class CreateAdminAccountRequest
    {
        [Required]
        [MinLength(1)]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public string Token { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
