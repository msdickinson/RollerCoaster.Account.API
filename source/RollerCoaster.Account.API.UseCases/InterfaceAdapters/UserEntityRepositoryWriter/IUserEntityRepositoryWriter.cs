using RollerCoaster.Account.API.Entities.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter
{
    public interface IUserEntityRepositoryWriter
    {
        Task<UserEntityData> SaveAsync(UserEntityData userEntityData);
    }
}
