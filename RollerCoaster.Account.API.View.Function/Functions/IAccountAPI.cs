using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Functions
{
    public interface IAccountAPI
    {
        Task<IActionResult> ActivateEmailAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/ActivateEmail")] HttpRequest httpRequest);
        Task<IActionResult> CreateAdminAccountAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "post" }, Route = "v1/Account/CreateAdminAccount")] HttpRequest httpRequest);
        Task<IActionResult> CreateUserAccountAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "post" }, Route = "v1/Account/CreateUserAccount")] HttpRequest httpRequest);
        Task<IActionResult> LoginAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/Login")] HttpRequest httpRequest);
        Task<IActionResult> RefreshTokensAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/RefreshTokens")] HttpRequest httpRequest);
        Task<IActionResult> RequestPasswordResetEmailAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/RequestPasswordResetEmail")] HttpRequest httpRequest);
        Task<IActionResult> ResetPasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/ResetPassword")] HttpRequest httpRequest);
        Task<IActionResult> UpdateEmailPreferenceAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/UpdateEmailPreference")] HttpRequest httpRequest);
        Task<IActionResult> UpdateEmailPreferenceWithTokenAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/UpdateEmailPreferenceWithToken")] HttpRequest httpRequest);
        Task<IActionResult> UpdatePasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, new[] { "get", "post" }, Route = "v1/Account/UpdatePassword")] HttpRequest httpRequest);
    }
}