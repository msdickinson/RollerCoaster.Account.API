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
using RollerCoaster.Account.API.Abstractions;
using System.Linq;

namespace RollerCoaster.Account.API.View.Controllers
{
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
        internal readonly IJWTService<RollerCoasterJWTServiceOptions> _rollerCoasterJWTServices;

        public AccountController
        (
            IAccountManager accountManager,
            IDateTimeService dateTimeService,
            IJWTService<RollerCoasterJWTServiceOptions> rollerCoasterJWTServices
        )
        {
            _rollerCoasterJWTServices = rollerCoasterJWTServices;
            _accountManager = accountManager;
            _dateTimeService = dateTimeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createUserAccountRequest"></param>
        /// <returns></returns>
        [HttpPost("CreateUserAccount")]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUserAccountAsync([FromBody]CreateUserAccountRequest createUserAccountRequest)
        {
            var createAccountDescriptor =
                await _accountManager.CreateUserAsync
                (
                    createUserAccountRequest.Username,
                    createUserAccountRequest.Password,
                    createUserAccountRequest.Email
                );

            if (createAccountDescriptor.Result == CreateUserAccountResult.DuplicateUser)
            {
                return StatusCode(409);
            }

            var accountId = Convert.ToString(createAccountDescriptor.AccountId);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim(ClaimTypes.Role, Role.User)
            };

            var generateTokensDescriptor = _rollerCoasterJWTServices.GenerateTokens(claims);

            if (generateTokensDescriptor.Authorized == false)
            {
                return StatusCode(500);
            }

            return Ok(generateTokensDescriptor.Tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createAdminAccountRequest"></param>
        /// <returns></returns>
        [HttpPost("CreateAdminAccount")]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAdminAccountAsync([FromBody] CreateAdminAccountRequest createAdminAccountRequest)
        {
            var createAccountDescriptor =
                await _accountManager.CreateAdminAsync
                (
                    createAdminAccountRequest.Username,
                    createAdminAccountRequest.Token,
                    createAdminAccountRequest.Password,
                    createAdminAccountRequest.Email
                );

            if (createAccountDescriptor.Result == CreateAdminAccountResult.InvaildToken)
            {
                return StatusCode(401);
            }

            if (createAccountDescriptor.Result == CreateAdminAccountResult.DuplicateUser)
            {
                return StatusCode(409);
            }

            var accountId = Convert.ToString(createAccountDescriptor.AccountId);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim(ClaimTypes.Role, Role.Admin)
            };

            var generateTokensDescriptor = _rollerCoasterJWTServices.GenerateTokens(claims);

            if (generateTokensDescriptor.Authorized == false)
            {
                return StatusCode(500);
            }

            return Ok(generateTokensDescriptor.Tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var loginDescriptor =
                await _accountManager.LoginAsync(loginRequest.Username, loginRequest.Password);

            if (loginDescriptor.Result == LoginResult.InvaildPassword)
            {
                return StatusCode(401);
            }

            //TODO: Not Adding Failed Attempt to DB, This wont fire.
            if (loginDescriptor.Result == LoginResult.AccountLocked)
            {
                return StatusCode(403);
            }

            if (loginDescriptor.Result == LoginResult.AccountNotFound)
            {
                return StatusCode(404);
            }

            var accountId = Convert.ToString(loginDescriptor.AccountId);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim(ClaimTypes.Role, loginDescriptor.Role)
            };

            var generateTokensDescriptor = _rollerCoasterJWTServices.GenerateTokens(claims);

            if (generateTokensDescriptor.Authorized == false)
            {
                return StatusCode(500);
            }

            return Ok(generateTokensDescriptor.Tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RefreshTokens"></param>
        /// <returns></returns>     
        [AllowAnonymous]
        [HttpPost("RefreshTokens")]
        [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshTokensAsync([FromBody] RefreshTokensRequest refreshTokensRequest)
        {
            await Task.CompletedTask;

            var accessTokenClaims = _rollerCoasterJWTServices.GetPrincipal(refreshTokensRequest.AccessToken, false, TokenType.Access);
            var refreshTokenClaims = _rollerCoasterJWTServices.GetPrincipal(refreshTokensRequest.RefreshToken, true, TokenType.Refresh);

            if (accessTokenClaims == null ||
                refreshTokenClaims == null ||
                !accessTokenClaims.Identity.IsAuthenticated ||
                !refreshTokenClaims.Identity.IsAuthenticated ||
                accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value !=
                refreshTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value)
            {
                return StatusCode(401);
            }

            var generateTokensDescriptor = _rollerCoasterJWTServices.GenerateTokens(refreshTokenClaims.Claims);

            if (generateTokensDescriptor.Authorized == false)
            {
                return StatusCode(500);
            }

            return Ok(generateTokensDescriptor.Tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UpdateEmailPreference"></param>
        /// <returns></returns>     
        [HttpPost("UpdateEmailPreference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmailPreferenceAsync([FromBody] UpdateEmailPreferenceRequest updateEmailPreferenceRequest)
        {
            int accountId = Convert.ToInt32(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            await _accountManager.UpdateEmailPreferenceAsync(accountId, updateEmailPreferenceRequest.EmailPreference);

            return StatusCode(200);
        }

        [AllowAnonymous]
        [HttpPost("UpdateEmailPreferenceWithToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmailPreferenceWithTokenAsync([FromBody] UpdateEmailPreferenceWithTokenRequest updateEmailPreferenceWithTokenRequest)
        {
            var updateEmailPreferenceWithTokenResult =
                await _accountManager.UpdateEmailPreferenceWithTokenAsync(updateEmailPreferenceWithTokenRequest.Token, updateEmailPreferenceWithTokenRequest.EmailPreference);

            if (updateEmailPreferenceWithTokenResult == UpdateEmailPreferenceWithTokenResult.InvaildToken)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }
    }
}
