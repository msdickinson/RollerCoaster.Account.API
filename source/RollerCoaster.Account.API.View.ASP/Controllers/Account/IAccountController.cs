using Microsoft.AspNetCore.Mvc;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using System.Threading.Tasks;

namespace CleanArchitecture.View.Controllers.Account
{
    public interface IAccountController
    {
        Task<ActionResult> CreateUserAccountAsync([FromBody] CreateUserAccountRequest createUserAccountRequest);
    }
}