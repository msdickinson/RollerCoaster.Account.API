using Dickinsonbros.Middleware.Function;
using DickinsonBros.Logger.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using RollerCoaster.Account.API.View.Function.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Functions
{
    public class StatusAPI : IStatusAPI
    {
        internal const string USER = "User";
        internal const string ADMIN = "Admin";
        internal readonly ILoggingService<StatusAPI> _logger;
        internal readonly IMiddlewareService<RollerCoasterJWTServiceOptions> _middlewareService;
        internal readonly IFunctionHelperService _functionHelperService;

        public StatusAPI
        (
            ILoggingService<StatusAPI> logger,
            IMiddlewareService<RollerCoasterJWTServiceOptions> middlewareService,
            IFunctionHelperService functionHelperService
        )
        {
            _logger = logger;
            _middlewareService = middlewareService;
            _functionHelperService = functionHelperService;
        }

        [FunctionName("Log")]
        public async Task<IActionResult> LogAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/Status/Log")] HttpRequest httpRequest
        )
        {
            _logger.LogInformationRedacted($"{nameof(StatusAPI)} Test Log");

            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                    return _functionHelperService.StatusCode(200);
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("UserAuthorized")]
        public async Task<IActionResult> UserAuthorizedAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/Status/UserAuthorized")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeWithJWTAuthAsync
            (
                httpRequest.HttpContext,
                async (user) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                    return _functionHelperService.StatusCode(200);
                },
                USER
            ).ConfigureAwait(false);
        }

        [FunctionName("AdminAuthorized")]
        public async Task<IActionResult> AdminAuthorizedAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/Status/AdminAuthorized")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeWithJWTAuthAsync
            (
                httpRequest.HttpContext,
                async (user) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                    return _functionHelperService.StatusCode(200);
                },
                ADMIN
            ).ConfigureAwait(false);
        }

    }
}
