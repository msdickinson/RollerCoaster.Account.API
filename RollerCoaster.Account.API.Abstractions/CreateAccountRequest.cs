using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Acccount.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class CreateAccountRequest
    {
        [Required]
        [MinLength(1)]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
