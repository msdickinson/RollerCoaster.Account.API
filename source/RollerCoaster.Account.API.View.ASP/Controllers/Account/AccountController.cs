using DickinsonBros.DateTime.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using RollerCoaster.Account.API.View.ASP.Exceptions;
using RollerCoaster.Account.API.View.ASP.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.View.Controllers.Account
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase, IAccountController
    {
        internal const string INVAILD_EMAIL_FORMAT = "Invaild Email Format";
        internal const string INVAILD_EMAIL_DOMAIN = "Invaild Email Domain";
        internal const string EMAIL = "Email";
        internal const string USERNAME = "Username";

        internal readonly IDateTimeService _dateTimeService;
        internal readonly ICreateUserAccountInteractor _createUserAccountInteractor;
        internal readonly IJWTService<RollerCoasterJWTServiceOptions> _rollerCoasterJWTServices;

        public AccountController
        (
            ICreateUserAccountInteractor createUserAccountInteractor,
            IDateTimeService dateTimeService,
            IJWTService<RollerCoasterJWTServiceOptions> rollerCoasterJWTServices
        )
        {
            _rollerCoasterJWTServices = rollerCoasterJWTServices;
            _createUserAccountInteractor = createUserAccountInteractor;
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
            try
            {
                var username =
                    await _createUserAccountInteractor.CreateUserAccountAsync
                    (
                        createUserAccountRequest
                    ).ConfigureAwait(false);

                var tokens = GenerateTokens(username);
                return Ok(tokens);
            }
            catch (InvaildRequestException invaildRequestsException)
            {
                return StatusCode(400, invaildRequestsException.InvaildRequestDatas);
            }
            catch (InvaildEmailFormatException)
            {
                return StatusCode(400, INVAILD_EMAIL_FORMAT);
            }
            catch (InvaildEmailDomainException)
            {
                return StatusCode(400, INVAILD_EMAIL_DOMAIN);
            }
            catch (DuplicateEmailException)
            {
                return StatusCode(409, EMAIL);
            }
            catch (DuplicateUsernameException)
            {
                return StatusCode(409, USERNAME);
            }
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
    }

}
