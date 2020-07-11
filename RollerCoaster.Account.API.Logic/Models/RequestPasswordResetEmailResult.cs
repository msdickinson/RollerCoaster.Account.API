using System;
using System.Collections.Generic;
using System.Text;

namespace RollerCoaster.Account.API.Logic.Models
{
    public enum RequestPasswordResetEmailResult
    {
        Successful,
        EmailNotFound,
        EmailNotActivated,
        NoEmailSentDueToEmailPreference
    }
}
