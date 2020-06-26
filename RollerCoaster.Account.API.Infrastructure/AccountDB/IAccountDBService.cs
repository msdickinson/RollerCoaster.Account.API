using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB
{
    public interface IAccountDBService
    {
        Task<InsertAccountResult> InsertAccountAsync(InsertAccountRequest insertAccountRequest);
    }
}