using RollerCoaster.Account.API.Entities.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader
{
    public interface IUserEntityRepositoryReader
    {
        Task<UserEntityData> LoadAsync(string userId);
        Task<bool> UsernameExistsAsync(string userId);
        Task<bool> EmailExistsAsync(string email);
    }
}
