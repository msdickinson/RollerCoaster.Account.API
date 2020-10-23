using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Logic.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Logic
{
    public interface IAccountManager
    {
        Task<CreateUserAccountDescriptor> CreateUserAsync(string username, string password, string email);
        Task<CreateAdminAccountDescriptor> CreateAdminAsync(string username, string token, string password, string email);
        Task<LoginDescriptor> LoginAsync(string username, string password);
        Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference);
        Task<UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string token, EmailPreference emailPreference);
        Task<ActivateEmailResult> ActivateEmailAsync(string token);
        Task<UpdatePasswordResult> UpdatePasswordAsync(int accountId, string existingPassword, string newPassword);
        Task<ResetPasswordResult> ResetPasswordAsync(string token, string newPassword);
        Task<RequestPasswordResetEmailResult> RequestPasswordResetEmailAsync(string email);
    }
}