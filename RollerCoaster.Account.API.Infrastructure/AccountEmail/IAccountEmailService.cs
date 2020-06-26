using System;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.AccountEmail
{
    public interface IAccountEmailService
    {
        Task SendActivateEmailAsync(string email, string username, string activateToken, string emailPreferenceToken);
        Task SendPasswordResetEmailAsync(string email, string passwordResetToken, string emailPreferenceToken);
    }
}