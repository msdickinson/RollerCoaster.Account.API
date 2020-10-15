using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using System.Diagnostics.CodeAnalysis;
[assembly: WebJobsStartup(typeof(RollerCoaster.Account.Function.Startup.Startup))]
namespace RollerCoaster.Account.Function.Startup
{

    [ExcludeFromCodeCoverage]
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            throw new System.NotImplementedException();
        }
    }

}
