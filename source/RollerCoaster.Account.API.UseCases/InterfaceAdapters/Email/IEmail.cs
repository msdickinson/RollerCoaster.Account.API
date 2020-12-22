using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email.Models;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email
{
    public interface IEmail
    {
        Task VaildateEmailAsync(string email);
        Task SendActivateEmailAsync(SendActivateEmailRequest sendActivateEmailRequest);
    }
}
