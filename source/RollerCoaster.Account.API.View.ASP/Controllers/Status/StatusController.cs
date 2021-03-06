﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using DickinsonBros.Logger.Abstractions;

namespace RollerCoaster.Account.API.View.ASP.Controllers.Status
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class StatusController : ControllerBase, IStatusController
    {
        internal readonly ILoggingService<StatusController> _logger;

        public StatusController
        (
            ILoggingService<StatusController> logger
        )
        {
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Log")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LogAsync()
        {
            _logger.LogInformationRedacted($"{nameof(StatusController)} Test Log");

            await Task.CompletedTask;

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorized"></param>
        /// <returns></returns>     
        [Authorize(Roles = "User")]
        [HttpGet("UserAuthorized")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UserAuthorizedAsync()
        {
            await Task.CompletedTask;

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorized"></param>
        /// <returns></returns>     
        [Authorize(Roles = "Admin")]
        [HttpGet("AdminAuthorized")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AdminAuthorizedAsync()
        {
            await Task.CompletedTask;

            return Ok();
        }

    }
}
