using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using DickinsonBros.DateTime.Abstractions;
using RollerCoaster.Acccount.API.Abstractions;
using RollerCoaster.Account.API.Logic;
using System.Security.Claims;
using DickinsonBros.Encryption.JWT.Abstractions;
using RollerCoaster.Account.API.View.Models;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using RollerCoaster.Account.API.Logic.Models;

namespace DickinsonBros.Account.API.View.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        internal const int FIFTEEN_MIN_IN_SECONDS = 900;
        internal const int TWO_HOURS_IN_SECONDS = 7200;
        internal const string BEARER_TOKEN_TYPE = "Bearer";

        internal readonly IDateTimeService _dateTimeService;
        internal readonly IAccountManager _accountManager;
        internal readonly IJWTService<WebsiteJWTServiceOptions> _websiteJWTService;

        public AccountController
        (
            IAccountManager accountManager,
            IDateTimeService dateTimeService,
            IJWTService<WebsiteJWTServiceOptions> websiteJWTService
        )
        {
            _websiteJWTService = websiteJWTService;
            _accountManager = accountManager;
            _dateTimeService = dateTimeService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="createAccountRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateAsync([FromBody]CreateAccountRequest createAccountRequest)
        {
            var createAccountDescriptor =
                await _accountManager.CreateAsync
                (
                    createAccountRequest.Username,
                    createAccountRequest.Password,
                    createAccountRequest.Email
                );

            if (createAccountDescriptor.Result == CreateAccountResult.DuplicateUser)
            {
                return StatusCode(409);
            }

            var accountId = Convert.ToString(createAccountDescriptor.AccountId);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim(ClaimTypes.Role, "User")
            };

            var generateTokensDescriptor = _websiteJWTService.GenerateTokens(claims);

            if (generateTokensDescriptor.Authorized == false)
            {
                return StatusCode(500);
            }

            return Ok(generateTokensDescriptor.Tokens);
        }
    }
}
