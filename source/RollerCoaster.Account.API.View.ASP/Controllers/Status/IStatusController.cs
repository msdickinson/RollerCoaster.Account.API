using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.View.ASP.Controllers.Status
{
    public interface IStatusController
    {
        Task<ActionResult> AdminAuthorizedAsync();
        Task<ActionResult> LogAsync();
        Task<ActionResult> UserAuthorizedAsync();
    }
}