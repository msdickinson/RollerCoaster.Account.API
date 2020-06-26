using RollerCoaster.Account.API.Logic.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Logic
{
    public interface IAccountManager
    {
        Task<CreateAccountDescriptor> CreateAsync(string username, string password, string email);
    }
}