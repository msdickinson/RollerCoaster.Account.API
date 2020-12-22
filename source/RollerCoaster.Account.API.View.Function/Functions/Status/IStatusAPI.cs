using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Functions
{
    public interface IStatusAPI
    {
        Task<IActionResult> AdminAuthorizedAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get" }, Route = "v1/Status/AdminAuthorized")] HttpRequest httpRequest);
        Task<IActionResult> LogAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get" }, Route = "v1/Status/Log")] HttpRequest httpRequest);
        Task<IActionResult> UserAuthorizedAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get" }, Route = "v1/Status/UserAuthorized")] HttpRequest httpRequest);
    }
}