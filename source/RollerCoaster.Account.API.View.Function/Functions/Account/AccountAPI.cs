using Dickinsonbros.Middleware.Function;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using RollerCoaster.Account.API.View.Function.Exceptions;
using RollerCoaster.Account.API.View.Function.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.Function.Functions.Account
{
    public class AccountAPI : IAccountAPI
    {
        #region constants
        internal const string INVAILD_EMAIL_FORMAT = "Invaild Email Format";
        internal const string INVAILD_EMAIL_DOMAIN = "Invaild Email Domain";
        internal const string EMAIL = "Email";
        internal const string USERNAME = "Username";

        internal readonly ICreateUserAccountInteractor _createUserAccountInteractor;
        internal readonly IJWTService<RollerCoasterJWTServiceOptions> _rollerCoasterJWTServices;
        internal readonly IMiddlewareService<RollerCoasterJWTServiceOptions> _middlewareService;
        internal readonly IFunctionHelperService _functionHelperService;
        #endregion

        #region .ctor
        public AccountAPI
        (
            ICreateUserAccountInteractor createUserAccountInteractor,
            IJWTService<RollerCoasterJWTServiceOptions> rollerCoasterJWTService,
            IMiddlewareService<RollerCoasterJWTServiceOptions> middlewareService,
            IFunctionHelperService functionHelperService
        )
        {
            _createUserAccountInteractor = createUserAccountInteractor;
            _rollerCoasterJWTServices = rollerCoasterJWTService;
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
                    try
                    {
                        //Parse and validate request
                        var processRequestDescriptor = await _functionHelperService.ProcessRequestAsync<CreateUserAccountRequest>(httpRequest).ConfigureAwait(false);
                        if (!processRequestDescriptor.IsSuccessful)
                        {
                            return processRequestDescriptor.ContentResult;
                        }
                        var createUserAccountRequest = processRequestDescriptor.Data;

                        //Process request
                        var username =
                        await _createUserAccountInteractor.CreateUserAccountAsync
                        (
                            createUserAccountRequest
                        ).ConfigureAwait(false);

                        var tokens = GenerateTokens(username);
                        return _functionHelperService.StatusCode(200, tokens);
                    }
                    catch (InvaildRequestException invaildRequestsException)
                    {
                        return _functionHelperService.StatusCode(400, invaildRequestsException.InvaildRequestDatas);
                    }
                    catch (InvaildEmailFormatException)
                    {
                        return _functionHelperService.StatusCode(400, INVAILD_EMAIL_FORMAT);
                    }
                    catch (InvaildEmailDomainException)
                    {
                        return _functionHelperService.StatusCode(400, INVAILD_EMAIL_DOMAIN);
                    }
                    catch (DuplicateEmailException)
                    {
                        return _functionHelperService.StatusCode(409, EMAIL);
                    }
                    catch (DuplicateUsernameException)
                    {
                        return _functionHelperService.StatusCode(409, USERNAME);
                    }
                }
            ).ConfigureAwait(false);

        }

        internal Tokens GenerateTokens(string username)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, Role.User.ToString())
            };

            var generateTokensDescriptor = _rollerCoasterJWTServices.GenerateTokens(claims);

            if (generateTokensDescriptor.Authorized == false)
            {
                throw new GenerateTokensServerUnauthorized();
            }

            return generateTokensDescriptor.Tokens;
        }

        #endregion

    }
}
