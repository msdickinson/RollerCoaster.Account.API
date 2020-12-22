using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Functions.Account
{
    public interface IAccountAPI
    {
        Task<IActionResult> CreateUserAccountAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "post" }, Route = "v1/Account/CreateUserAccount")] HttpRequest httpRequest);
    }
}