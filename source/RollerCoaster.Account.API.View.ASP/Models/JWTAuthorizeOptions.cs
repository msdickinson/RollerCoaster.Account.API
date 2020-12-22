using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.View.ASP.Models
{
    [ExcludeFromCodeCoverage]
    public class JWTAuthorizeOptions
    {
        public string VaildIssuer { get; set; }
        public string VaildAudience { get; set; }
        public string StoreLocation { get; set; }
        public string ThumbPrint { get; set; }
    }
}
