using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.View.Models
{
    [ExcludeFromCodeCoverage]
    public class AWSElasticsearchOptions
    {  
        public string AWSRegion { get; set; }
        public string URL { get; set; }
        public string IndexFormat { get; set; }
    }
}
