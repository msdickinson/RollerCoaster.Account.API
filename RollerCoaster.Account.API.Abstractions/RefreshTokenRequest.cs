using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokensRequest
    {
        [Required]
        [MinLength(1)]
        public string AccessToken { get; set; }

        [Required]
        [MinLength(1)]
        public string RefreshToken { get; set; }
    }
}
