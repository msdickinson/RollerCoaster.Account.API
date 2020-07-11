using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
namespace RollerCoaster.Account.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class RequestPasswordResetEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
