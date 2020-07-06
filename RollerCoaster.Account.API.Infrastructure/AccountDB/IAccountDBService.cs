using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB
{
    public interface IAccountDBService
    {
        Task<InsertAccountResult> InsertAccountAsync(InsertAccountRequest insertAccountRequest);
        Task<Abstractions.Account> SelectAccountByUserNameAsync(SelectAccountByUserNameRequest selectAccountByUserNameRequest);
        Task InsertPasswordAttemptFailedAsync(InsertPasswordAttemptFailedRequest insertPasswordAttemptFailedRequest);
        Task UpdateEmailPreferenceAsync(Models.UpdateEmailPreferenceRequest updateEmailPreferenceRequest);
        Task<Models.UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(Models.UpdateEmailPreferenceWithTokenRequest updateEmailPreferenceWithTokenRequest);
    }
}