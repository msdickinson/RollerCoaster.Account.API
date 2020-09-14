using DickinsonBros.DateTime.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RollerCoaster.Acccount.API.Abstractions;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Logic;
using RollerCoaster.Account.API.Logic.Models;
using RollerCoaster.Account.API.View.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        internal const string EMAIL_NOT_ACTIVATED_MESSAGE = "Email Not Activated";
        internal const string NO_EMAIL_SENT_DUE_TO_EMAIL_PERFERENCEMESSAGE = "No Email Sent Due To Email Preference";
        internal const string EMAIL_HAS_ALREADY_BEEN_ACTIVATED = "Email has already been activated";

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
        [AllowAnonymous]
        [HttpPost("CreateUserAccount")]
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUserAccountAsync([FromBody] CreateUserAccountRequest createUserAccountRequest)
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        /// 
        [Authorize(Roles = Role.Admin + "," + Role.User)]
        [HttpPost("UpdateEmailPreference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmailPreferenceAsync([FromBody] UpdateEmailPreferenceRequest updateEmailPreferenceRequest)
        {
            int accountId = Convert.ToInt32(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            await _accountManager.UpdateEmailPreferenceAsync(accountId, updateEmailPreferenceRequest.EmailPreference);

            return StatusCode(200);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UpdateEmailPreferenceWithToken"></param>
        /// <returns></returns>  
        [AllowAnonymous]
        [HttpPost("UpdateEmailPreferenceWithToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmailPreferenceWithTokenAsync([FromBody] UpdateEmailPreferenceWithTokenRequest updateEmailPreferenceWithTokenRequest)
        {
            if(!Guid.TryParse(updateEmailPreferenceWithTokenRequest.Token, out _))
            {
                return StatusCode(400);
            }

            var updateEmailPreferenceWithTokenResult =
                await _accountManager.UpdateEmailPreferenceWithTokenAsync(updateEmailPreferenceWithTokenRequest.Token.ToString(), updateEmailPreferenceWithTokenRequest.EmailPreference);

            if (updateEmailPreferenceWithTokenResult == UpdateEmailPreferenceWithTokenResult.InvaildToken)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActivateEmail"></param>
        /// <returns></returns>     
        [AllowAnonymous]
        [HttpPost("ActivateEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActivateEmailAsync([FromBody] ActivateEmailRequest activateEmailRequest)
        {
            if (!Guid.TryParse(activateEmailRequest.Token, out _))
            {
                return StatusCode(400);
            }

            var activateAccountResult =
                await _accountManager.ActivateEmailAsync(activateEmailRequest.Token);

            if (activateAccountResult == ActivateEmailResult.InvaildToken)
            {
                return StatusCode(401);
            }

            if (activateAccountResult == ActivateEmailResult.EmailWasAlreadyActivated)
            {
                return StatusCode(400, "Email has already been activated");
            }
            return StatusCode(200);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UpdatePassword"></param>
        /// <returns></returns>     
        [Authorize(Roles = Role.Admin + "," + Role.User)]
        [HttpPost("UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            int accountId = Convert.ToInt32(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            var updatePasswordResult =
                await _accountManager.UpdatePasswordAsync(accountId, updatePasswordRequest.ExistingPassword, updatePasswordRequest.NewPassword).ConfigureAwait(false);

            if (updatePasswordResult == UpdatePasswordResult.AccountLocked)
            {
                return StatusCode(403);
            }

            if (updatePasswordResult == UpdatePasswordResult.InvaildExistingPassword)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResetPassword"></param>
        /// <returns></returns>     
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            if (!Guid.TryParse(resetPasswordRequest.Token, out _))
            {
                return StatusCode(400);
            }

            var resetPasswordResult =
                await _accountManager.ResetPasswordAsync(resetPasswordRequest.Token, resetPasswordRequest.NewPassword);

            if (resetPasswordResult == ResetPasswordResult.TokenInvaild)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }

        [AllowAnonymous]
        [HttpPost("RequestPasswordResetEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestPasswordResetEmailAsync([FromBody] RequestPasswordResetEmailRequest requestPasswordResetEmailRequest)
        {
            var requestPasswordResetEmailResult =
                await _accountManager.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest.Email);

            if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.EmailNotFound)
            {
                return StatusCode(404);
            }

            if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.EmailNotActivated)
            {
                return StatusCode(403, EMAIL_NOT_ACTIVATED_MESSAGE);
            }

            if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference)
            {
                return StatusCode(403, NO_EMAIL_SENT_DUE_TO_EMAIL_PERFERENCEMESSAGE);
            }

            return StatusCode(200);
        }
    }
}
