using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DickinsonBros.DateTime.Abstractions;
using RollerCoaster.Account.API.Logic;
using DickinsonBros.Encryption.JWT.Abstractions;
using RollerCoaster.Account.API.View.Models;
using RollerCoaster.Account.API.Abstractions;
using System.Security.Claims;

namespace RollerCoaster.Account.Function
{
    public class AccountFunction
    {
        #region constants
        internal const int FIFTEEN_MIN_IN_SECONDS = 900;
        internal const int TWO_HOURS_IN_SECONDS = 7200;
        internal const string BEARER_TOKEN_TYPE = "Bearer";
        internal const string EMAIL_NOT_ACTIVATED_MESSAGE = "Email Not Activated";
        internal const string NO_EMAIL_SENT_DUE_TO_EMAIL_PERFERENCEMESSAGE = "No Email Sent Due To Email Preference";
        internal const string EMAIL_HAS_ALREADY_BEEN_ACTIVATED = "Email has already been activated";

        internal readonly IDateTimeService _dateTimeService;
        internal readonly IAccountManager _accountManager;
        internal readonly IJWTService<RollerCoasterJWTServiceOptions> _rollerCoasterJWTServices;
        #endregion

        #region .ctor
        public AccountFunction
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
        #endregion

        #region function

        //JWT Middleware
        //Logging + Corrleation Id Middleware
        //Swagger
        [FunctionName("LoginFunction")]
        public async Task<IActionResult> LoginAsync
        (
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Login")] [FromBody] LoginRequest loginRequest
        )
        {
            var loginDescriptor =
                await _accountManager.LoginAsync(loginRequest.Username, loginRequest.Password);

            if (loginDescriptor.Result == LoginResult.InvaildPassword)
            {
                return new StatusCodeResult(401);
            }

            if (loginDescriptor.Result == LoginResult.AccountLocked)
            {
                return new StatusCodeResult(403);
            }

            if (loginDescriptor.Result == LoginResult.AccountNotFound)
            {
                return new StatusCodeResult(404);
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
                return new StatusCodeResult(500);
            }

            return new OkObjectResult(generateTokensDescriptor.Tokens);
        }

        #endregion
    }
}
