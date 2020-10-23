using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class UpdateEmailPreferenceRequest
    {
        [Required]
        [EnumDataType(typeof(EmailPreference))]
        public EmailPreference EmailPreference { get; set; }
    }
}
