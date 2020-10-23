using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB
{
    public interface IAccountDBService
    {
        Task<ActivateEmailWithTokenResult> ActivateEmailWithTokenAsync(string activateEmailToken);
        Task<InsertAccountResult> InsertAccountAsync(InsertAccountRequest insertAccountRequest);
        Task InsertPasswordResetTokenAsync(int accountId, string passwordResetToken);
        Task<Abstractions.Account> SelectAccountByAccountIdAsync(int accountId);
        Task<Abstractions.Account> SelectAccountByEmailAsync(string email);
        Task<Abstractions.Account> SelectAccountByUserNameAsync(string username);
        Task<int?> SelectAccountIdFromPasswordResetTokenAsync(string passwordResetToken);
        Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference);
        Task<Models.UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string emailPreferenceToken, EmailPreference emailPreference);
        Task UpdatePasswordAsync(int accountId, string passwordHash, string salt);
        Task InsertPasswordAttemptFailedAsync(int accountId);
     
    }
}