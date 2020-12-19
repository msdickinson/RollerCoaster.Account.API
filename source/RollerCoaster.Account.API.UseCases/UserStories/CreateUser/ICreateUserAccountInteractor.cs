using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.UseCases.UserStorys.CreateUser
{
    public interface ICreateUserAccountInteractor
    {
        Task<string> CreateUserAccountAsync(CreateUserAccountRequest createUserRequest);
    }
}