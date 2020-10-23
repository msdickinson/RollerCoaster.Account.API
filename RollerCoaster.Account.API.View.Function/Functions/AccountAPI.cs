using Dickinsonbros.Middleware.Function;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using RollerCoaster.Acccount.API.Abstractions;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.View.Function.Models;
using RollerCoaster.Account.API.Logic;
using RollerCoaster.Account.API.Logic.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Dickinsonbros.Middleware.Function.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RollerCoaster.Account.API.View.Function.Functions
{
    public class AccountAPI : IAccountAPI
    {
        #region constants
        internal const int FIFTEEN_MIN_IN_SECONDS = 900;
        internal const int TWO_HOURS_IN_SECONDS = 7200;
        internal const string BEARER_TOKEN_TYPE = "Bearer";
        internal const string EMAIL_NOT_ACTIVATED_MESSAGE = "Email Not Activated";
        internal const string NO_EMAIL_SENT_DUE_TO_EMAIL_PERFERENCEMESSAGE = "No Email Sent Due To Email Preference";
        internal const string EMAIL_HAS_ALREADY_BEEN_ACTIVATED = "Email has already been activated";
        internal const string INVAILD_EMAIL_FORMAT = "Invaild Email Format";
        internal const string INVAILD_EMAIL_DOMAIN = "Invaild Email Domain";
        internal const string USER = "User";
        internal const string ADMIN = "Admin";

        internal readonly IAccountManager _accountManager;
        internal readonly IJWTService<RollerCoasterJWTServiceOptions> _rollerCoasterJWTServices;
        internal readonly IMiddlewareService<RollerCoasterJWTServiceOptions> _middlewareService;
        internal readonly IFunctionHelperService _functionHelperService;
        #endregion

        #region .ctor
        public AccountAPI
        (
            IAccountManager accountManager,
            IJWTService<RollerCoasterJWTServiceOptions> rollerCoasterJWTServices,
            IMiddlewareService<RollerCoasterJWTServiceOptions> middlewareService,
            IFunctionHelperService functionHelperService
        )
        {
            _rollerCoasterJWTServices = rollerCoasterJWTServices;
            _accountManager = accountManager;
            _middlewareService = middlewareService;
            _functionHelperService = functionHelperService;
        }
        #endregion

        #region function

        [FunctionName("CreateUserAccountFunction")]
        public async Task<IActionResult> CreateUserAccountAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/Account/CreateUserAccount")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<CreateUserAccountRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var createUserAccountRequest = processRequestDescriptor.Data;

                    //Process request
                    var createAccountDescriptor =
                    await _accountManager.CreateUserAsync
                    (
                        createUserAccountRequest.Username,
                        createUserAccountRequest.Password,
                        createUserAccountRequest.Email
                    );

                    if (createAccountDescriptor.Result == CreateUserAccountResult.InvaildEmailFormat)
                    {
                        return _functionHelperService.StatusCode(400, INVAILD_EMAIL_FORMAT);
                    }

                    if (createAccountDescriptor.Result == CreateUserAccountResult.InvaildEmailDomain)
                    {
                        return _functionHelperService.StatusCode(400, INVAILD_EMAIL_DOMAIN);
                    }

                    if (createAccountDescriptor.Result == CreateUserAccountResult.DuplicateUser)
                    {
                        return _functionHelperService.StatusCode(409);
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
                        return _functionHelperService.StatusCode(500);
                    }

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = JsonSerializer.Serialize(generateTokensDescriptor.Tokens),
                        ContentType = "application/json"
                    };
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("CreateAdminAccountFunction")]
        public async Task<IActionResult> CreateAdminAccountAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/Account/CreateAdminAccount")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<CreateAdminAccountRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var createAdminAccountRequest = processRequestDescriptor.Data;

                    //Process request
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
                        return _functionHelperService.StatusCode(401);
                    }

                    if (createAccountDescriptor.Result == CreateAdminAccountResult.InvaildEmailFormat)
                    {
                        return _functionHelperService.StatusCode(400, INVAILD_EMAIL_FORMAT);
                    }

                    if (createAccountDescriptor.Result == CreateAdminAccountResult.InvaildEmailDomain)
                    {
                        return _functionHelperService.StatusCode(400, INVAILD_EMAIL_DOMAIN);
                    }

                    if (createAccountDescriptor.Result == CreateAdminAccountResult.DuplicateUser)
                    {
                        return _functionHelperService.StatusCode(409);
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
                        return _functionHelperService.StatusCode(500);
                    }

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = JsonSerializer.Serialize(generateTokensDescriptor.Tokens),
                        ContentType = "application/json"
                    };
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("LoginFunction")]
        public async Task<IActionResult> LoginAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/Login")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<LoginRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var loginRequest = processRequestDescriptor.Data;

                    //Process request
                    var loginDescriptor =
                        await _accountManager.LoginAsync(loginRequest.Username, loginRequest.Password);

                    if (loginDescriptor.Result == LoginResult.InvaildPassword)
                    {
                        return _functionHelperService.StatusCode(401);
                    }

                    if (loginDescriptor.Result == LoginResult.AccountLocked)
                    {
                        return _functionHelperService.StatusCode(403);
                    }

                    if (loginDescriptor.Result == LoginResult.AccountNotFound)
                    {
                        return _functionHelperService.StatusCode(404);
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
                        return _functionHelperService.StatusCode(500);
                    }

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = JsonSerializer.Serialize(generateTokensDescriptor.Tokens),
                        ContentType = "application/json"
                    };
                }
            ).ConfigureAwait(false);
        }


        [FunctionName("RefreshTokensFunction")]
        public async Task<IActionResult> RefreshTokensAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/RefreshTokens")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<RefreshTokensRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var refreshTokensRequest = processRequestDescriptor.Data;

                    //Process request
                    var accessTokenClaims = _rollerCoasterJWTServices.GetPrincipal(refreshTokensRequest.AccessToken, false, TokenType.Access);
                    var refreshTokenClaims = _rollerCoasterJWTServices.GetPrincipal(refreshTokensRequest.RefreshToken, true, TokenType.Refresh);

                    if (accessTokenClaims == null ||
                        refreshTokenClaims == null ||
                        !accessTokenClaims.Identity.IsAuthenticated ||
                        !refreshTokenClaims.Identity.IsAuthenticated ||
                        accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value !=
                        refreshTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value)
                    {
                        return _functionHelperService.StatusCode(401);
                    }

                    var generateTokensDescriptor = _rollerCoasterJWTServices.GenerateTokens(refreshTokenClaims.Claims);

                    if (generateTokensDescriptor.Authorized == false)
                    {
                        return _functionHelperService.StatusCode(500);
                    }

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = JsonSerializer.Serialize(generateTokensDescriptor.Tokens),
                        ContentType = "application/json"
                    };
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("UpdateEmailPreferenceFunction")]
        public async Task<IActionResult> UpdateEmailPreferenceAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/UpdateEmailPreference")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeWithJWTAuthAsync
            (
                httpRequest.HttpContext,
                async (user) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<UpdateEmailPreferenceRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var updateEmailPreferenceRequest = processRequestDescriptor.Data;

                    //Process request
                    int accountId = Convert.ToInt32(user.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

                    await _accountManager.UpdateEmailPreferenceAsync(accountId, updateEmailPreferenceRequest.EmailPreference);

                    return _functionHelperService.StatusCode(200);
                },
                USER,
                ADMIN
            ).ConfigureAwait(false);
        }

        [FunctionName("UpdateEmailPreferenceWithTokenFunction")]
        public async Task<IActionResult> UpdateEmailPreferenceWithTokenAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/UpdateEmailPreferenceWithToken")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<UpdateEmailPreferenceWithTokenRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var updateEmailPreferenceWithTokenRequest = processRequestDescriptor.Data;

                    //Process request
                    if (!Guid.TryParse(updateEmailPreferenceWithTokenRequest.Token, out _))
                    {
                        return _functionHelperService.StatusCode(400);
                    }

                    var updateEmailPreferenceWithTokenResult =
                        await _accountManager.UpdateEmailPreferenceWithTokenAsync(updateEmailPreferenceWithTokenRequest.Token.ToString(), updateEmailPreferenceWithTokenRequest.EmailPreference);

                    if (updateEmailPreferenceWithTokenResult == UpdateEmailPreferenceWithTokenResult.InvaildToken)
                    {
                        return _functionHelperService.StatusCode(401);
                    }

                    return _functionHelperService.StatusCode(200);
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("ActivateEmailFunction")]
        public async Task<IActionResult> ActivateEmailAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/ActivateEmail")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<ActivateEmailRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var activateEmailRequest = processRequestDescriptor.Data;

                    //Process request
                    if (!Guid.TryParse(activateEmailRequest.Token, out _))
                    {
                        return _functionHelperService.StatusCode(400);
                    }

                    var activateAccountResult =
                        await _accountManager.ActivateEmailAsync(activateEmailRequest.Token);

                    if (activateAccountResult == ActivateEmailResult.InvaildToken)
                    {
                        return _functionHelperService.StatusCode(401);
                    }

                    if (activateAccountResult == ActivateEmailResult.EmailWasAlreadyActivated)
                    {
                        return _functionHelperService.StatusCode(400, "Email has already been activated");
                    }

                    return _functionHelperService.StatusCode(200);
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("UpdatePasswordFunction")]
        public async Task<IActionResult> UpdatePasswordAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/UpdatePassword")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeWithJWTAuthAsync
            (
                httpRequest.HttpContext,
                async (user) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<UpdatePasswordRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var updatePasswordRequest = processRequestDescriptor.Data;

                    //Process request
                    int accountId = Convert.ToInt32(user.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

                    var updatePasswordResult =
                        await _accountManager.UpdatePasswordAsync(accountId, updatePasswordRequest.ExistingPassword, updatePasswordRequest.NewPassword).ConfigureAwait(false);

                    if (updatePasswordResult == UpdatePasswordResult.AccountLocked)
                    {
                        return _functionHelperService.StatusCode(403);
                    }

                    if (updatePasswordResult == UpdatePasswordResult.InvaildExistingPassword)
                    {
                        return _functionHelperService.StatusCode(401);
                    }

                    return _functionHelperService.StatusCode(200);
                },
                USER,
                ADMIN
            ).ConfigureAwait(false);
        }

        [FunctionName("ResetPasswordFunction")]
        public async Task<IActionResult> ResetPasswordAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/ResetPassword")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<ResetPasswordRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var resetPasswordRequest = processRequestDescriptor.Data;

                    //Process request
                    if (!Guid.TryParse(resetPasswordRequest.Token, out _))
                    {
                        return _functionHelperService.StatusCode(400);
                    }

                    var resetPasswordResult =
                        await _accountManager.ResetPasswordAsync(resetPasswordRequest.Token, resetPasswordRequest.NewPassword);

                    if (resetPasswordResult == ResetPasswordResult.TokenInvaild)
                    {
                        return _functionHelperService.StatusCode(401);
                    }

                    return _functionHelperService.StatusCode(200);
                }
            ).ConfigureAwait(false);
        }

        [FunctionName("RequestPasswordResetEmail")]
        public async Task<IActionResult> RequestPasswordResetEmailAsync
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/Account/RequestPasswordResetEmail")] HttpRequest httpRequest
        )
        {
            return await _middlewareService.InvokeAsync
            (
                httpRequest.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    //Parse and validate request
                    var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<RequestPasswordResetEmailRequest>(httpRequest).ConfigureAwait(false);
                    if (!processRequestDescriptor.IsSuccessful)
                    {
                        return processRequestDescriptor.ContentResult;
                    }
                    var requestPasswordResetEmailRequest = processRequestDescriptor.Data;

                    //Process request
                    var requestPasswordResetEmailResult =
                        await _accountManager.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest.Email);

                    if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.EmailNotFound)
                    {
                        return _functionHelperService.StatusCode(404);
                    }

                    if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.EmailNotActivated)
                    {
                        return _functionHelperService.StatusCode(403, EMAIL_NOT_ACTIVATED_MESSAGE);
                    }

                    if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference)
                    {
                        return _functionHelperService.StatusCode(403, NO_EMAIL_SENT_DUE_TO_EMAIL_PERFERENCEMESSAGE);
                    }

                    return _functionHelperService.StatusCode(200);
                }
            ).ConfigureAwait(false);
        }
        #endregion

    }
}
